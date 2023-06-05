using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using Microsoft.Build.Utilities;

// Helper class to use 2.0 root config as necessary. Each helper method will
// go through RuntimeConfig (the original code path), except in the
// case of building and targeting 2.0/3.5.
internal class MTConfigUtil
{
    private static readonly ConcurrentDictionary<
        Tuple<Type, VirtualPath>,
        ConfigurationSection
    > s_sections = new ConcurrentDictionary<Tuple<Type, VirtualPath>, ConfigurationSection>();

    private static readonly ConcurrentDictionary<VirtualPath, Configuration> s_configurations =
        new ConcurrentDictionary<VirtualPath, Configuration>();

    private static string s_machineConfigPath;
    private static VirtualPath s_appVirtualPath;

    private
    // We only need to use the root config of 2.0 if we are building (and
    // not during runtime) and targeting 2.0 or 3.5.
    static bool? s_useMTConfig;
    private static bool UseMTConfig
    {
        get
        {
            if (s_useMTConfig == null)
            {
                s_useMTConfig =
                    BuildManagerHost.InClientBuildManager
                    && (
                        MultiTargetingUtil.IsTargetFramework20
                        || MultiTargetingUtil.IsTargetFramework35
                    );
            }
            return s_useMTConfig.Value;
        }
    }

    // Counterpart for RuntimeConfig.GetAppConfig().Profile;
    internal
    // Counterpart for RuntimeConfig.GetAppConfig().Profile;
    static ProfileSection GetProfileAppConfig()
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetAppConfig().Profile;
        }
        return GetAppConfig<ProfileSection>();
    }

    // Counterpart for RuntimeConfig.GetAppConfig().Pages;
    internal
    // Counterpart for RuntimeConfig.GetAppConfig().Pages;
    static PagesSection GetPagesAppConfig()
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetAppConfig().Pages;
        }
        return GetAppConfig<PagesSection>();
    }

    // Counterpart for RuntimeConfig.GetConfig().Pages;
    internal
    // Counterpart for RuntimeConfig.GetConfig().Pages;
    static PagesSection GetPagesConfig()
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetConfig().Pages;
        }
        return GetConfig<PagesSection>();
    }

    // Counterpart for RuntimeConfig.GetConfig(string).Pages
    internal
    // Counterpart for RuntimeConfig.GetConfig(string).Pages
    static PagesSection GetPagesConfig(string vpath)
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetConfig(vpath).Pages;
        }
        return GetConfig<PagesSection>(vpath);
    }

    // Counterpart for RuntimeConfig.GetConfig(VirtualPath).Pages
    internal
    // Counterpart for RuntimeConfig.GetConfig(VirtualPath).Pages
    static PagesSection GetPagesConfig(VirtualPath vpath)
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetConfig(vpath).Pages;
        }
        return GetConfig<PagesSection>(vpath);
    }

    // Counterpart for RuntimeConfig.GetConfig(HttpContext).Pages
    internal
    // Counterpart for RuntimeConfig.GetConfig(HttpContext).Pages
    static PagesSection GetPagesConfig(HttpContext context)
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetConfig(context).Pages;
        }
        return GetConfig<PagesSection>(context);
    }

    // Counterpart for RuntimeConfig.GetConfig().Compilation
    internal
    // Counterpart for RuntimeConfig.GetConfig().Compilation
    static CompilationSection GetCompilationConfig()
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetConfig().Compilation;
        }
        return GetConfig<CompilationSection>();
    }

    // Counterpart for RuntimeConfig.GetAppConfig().Compilation
    internal
    // Counterpart for RuntimeConfig.GetAppConfig().Compilation
    static CompilationSection GetCompilationAppConfig()
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetAppConfig().Compilation;
        }
        return GetAppConfig<CompilationSection>();
    }

    // Counterpart for RuntimeConfig.GetConfig(string).Compilation
    internal
    // Counterpart for RuntimeConfig.GetConfig(string).Compilation
    static CompilationSection GetCompilationConfig(string vpath)
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetConfig(vpath).Compilation;
        }
        return GetConfig<CompilationSection>(vpath);
    }

    // Counterpart for RuntimeConfig.GetConfig(VirtualPath).Compilation
    internal
    // Counterpart for RuntimeConfig.GetConfig(VirtualPath).Compilation
    static CompilationSection GetCompilationConfig(VirtualPath vpath)
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetConfig(vpath).Compilation;
        }
        return GetConfig<CompilationSection>(vpath);
    }

    // Counterpart for RuntimeConfig.GetConfig(HttpContext).Compilation
    internal
    // Counterpart for RuntimeConfig.GetConfig(HttpContext).Compilation
    static CompilationSection GetCompilationConfig(HttpContext context)
    {
        if (!UseMTConfig)
        {
            return RuntimeConfig.GetConfig(context).Compilation;
        }
        return GetConfig<CompilationSection>(context);
    }

    // Counterpart to RuntimeConfig.GetConfig()
    private
    // Counterpart to RuntimeConfig.GetConfig()
    static S GetConfig<S>()
        where S : ConfigurationSection
    {
        HttpContext context = HttpContext.Current;
        if (context != null)
        {
            return GetConfig<S>(context);
        }
        else
        {
            return GetAppConfig<S>();
        }
    }

    // Counterpart to RuntimeConfig.GetAppConfig()
    private
    // Counterpart to RuntimeConfig.GetAppConfig()
    static S GetAppConfig<S>()
        where S : ConfigurationSection
    {
        return GetConfig<S>((VirtualPath)null);
    }

    // Counterpart to RuntimeConfig.GetConfig(HttpContext)
    private
    // Counterpart to RuntimeConfig.GetConfig(HttpContext)
    static S GetConfig<S>(HttpContext context)
        where S : ConfigurationSection
    {
        return GetConfig<S>(context.ConfigurationPath);
    }

    // Counterpart to RuntimeConfig.GetConfig(string)
    private
    // Counterpart to RuntimeConfig.GetConfig(string)
    static S GetConfig<S>(string vpath)
        where S : ConfigurationSection
    {
        return GetConfig<S>(VirtualPath.CreateNonRelativeAllowNull(vpath));
    }

    // Counterpart to RuntimeConfig.GetConfig(VirtualPath)
    private
    // Counterpart to RuntimeConfig.GetConfig(VirtualPath)
    static S GetConfig<S>(VirtualPath vpath)
        where S : ConfigurationSection
    {
        Tuple<Type, VirtualPath> key = new Tuple<Type, VirtualPath>(typeof(S), vpath);
        ConfigurationSection result;
        if (!s_sections.TryGetValue(key, out result))
        {
            result = GetConfigHelper<S>(vpath);
            s_sections.TryAdd(key, result);
        }
        return result as S;
    }

    // Actual method performing to work to retrieve the required ConfigurationSection.
    private
    // Actual method performing to work to retrieve the required ConfigurationSection.
    static S GetConfigHelper<S>(VirtualPath vpath)
        where S : ConfigurationSection
    {
        string physicalPath = null;
        if (vpath == null || !vpath.IsWithinAppRoot)
        {
            // If virtual path is null or outside the application root, we use the application level config.
            vpath = HostingEnvironment.ApplicationVirtualPathObject;
            physicalPath = HostingEnvironment.ApplicationPhysicalPath;
        }
        else
        {
            // If it is not a directory, use the directory as the vpath
            if (!vpath.DirectoryExists())
            {
                vpath = vpath.Parent;
            }
            physicalPath = HostingEnvironment.MapPath(vpath);
        }

        Configuration config = GetConfiguration(vpath, physicalPath);

        // Retrieve the specified section
        if (typeof(S) == typeof(CompilationSection))
        {
            return config.GetSection("system.web/compilation") as S;
        }
        else if (typeof(S) == typeof(PagesSection))
        {
            return config.GetSection("system.web/pages") as S;
        }
        else if (typeof(S) == typeof(ProfileSection))
        {
            return config.GetSection("system.web/profile") as S;
        }

        throw new InvalidOperationException(
            SR.GetString(SR.Config_section_not_supported, typeof(S).FullName)
        );
    }

    private static string MachineConfigPath
    {
        get
        {
            if (s_machineConfigPath == null)
            {
                s_machineConfigPath = ToolLocationHelper.GetPathToDotNetFrameworkFile(
                    @"config\machine.config",
                    TargetDotNetFrameworkVersion.Version20
                );
                if (string.IsNullOrEmpty(s_machineConfigPath))
                {
                    string message = SR.GetString(SR.Downlevel_requires_35);
                    throw new InvalidOperationException(message);
                }
            }
            return s_machineConfigPath;
        }
    }

    private static Configuration GetConfiguration(VirtualPath vpath, string physicalPath)
    {
        Configuration result;
        if (!s_configurations.TryGetValue(vpath, out result))
        {
            result = GetConfigurationHelper(vpath, physicalPath);
            s_configurations.TryAdd(vpath, result);
        }
        return result;
    }

    private static Configuration GetConfigurationHelper(VirtualPath vpath, string physicalPath)
    {
        // Set up the configuration file map
        string machineConfigPath = MachineConfigPath;
        WebConfigurationFileMap fileMap = new WebConfigurationFileMap(machineConfigPath);

        // Set up file maps for the current directory and parent directories up to application root
        VirtualPath currentVPath = vpath;
        while (currentVPath != null && currentVPath.IsWithinAppRoot)
        {
            string vpathString = currentVPath.VirtualPathStringNoTrailingSlash;
            if (physicalPath == null)
            {
                physicalPath = HostingEnvironment.MapPath(currentVPath);
            }
            fileMap.VirtualDirectories.Add(
                vpathString,
                new VirtualDirectoryMapping(physicalPath, IsAppRoot(currentVPath))
            );
            currentVPath = currentVPath.Parent;
            physicalPath = null;
        }

        Configuration config = WebConfigurationManager.OpenMappedWebConfiguration(
            fileMap,
            vpath.VirtualPathStringNoTrailingSlash,
            HostingEnvironment.SiteName
        );
        return config;
    }

    private static bool IsAppRoot(VirtualPath path)
    {
        if (s_appVirtualPath == null)
        {
            s_appVirtualPath = VirtualPath.Create(
                HttpRuntime.AppDomainAppVirtualPathObject.VirtualPathStringNoTrailingSlash
            );
        }

        var vpath = VirtualPath.Create(path.VirtualPathStringNoTrailingSlash);
        return s_appVirtualPath.Equals(vpath);
    }
}
