// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ------------------------------------------------------------------------------
// Changes to this file must follow the https://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace Microsoft.Extensions.FileSystemGlobbing
{
    partial public struct FilePatternMatch
        : System.IEquatable<Microsoft.Extensions.FileSystemGlobbing.FilePatternMatch>
    {
        private object _dummy;
        private int _dummyPrimitive;

        public FilePatternMatch(string path, string? stem)
        {
            throw null;
        }

        public readonly string Path
        {
            get { throw null; }
        }
        public readonly string? Stem
        {
            get { throw null; }
        }

        public bool Equals(Microsoft.Extensions.FileSystemGlobbing.FilePatternMatch other)
        {
            throw null;
        }

        public override bool Equals(
            [System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj
        )
        {
            throw null;
        }

        public override int GetHashCode()
        {
            throw null;
        }
    }

    partial public class InMemoryDirectoryInfo
        : Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase
    {
        public InMemoryDirectoryInfo(
            string rootDir,
            System.Collections.Generic.IEnumerable<string>? files
        ) { }

        public override string FullName
        {
            get { throw null; }
        }
        public override string Name
        {
            get { throw null; }
        }
        public override Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? ParentDirectory
        {
            get { throw null; }
        }

        public override System.Collections.Generic.IEnumerable<Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileSystemInfoBase> EnumerateFileSystemInfos()
        {
            throw null;
        }

        public override Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase GetDirectory(
            string path
        )
        {
            throw null;
        }

        public override Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase? GetFile(
            string path
        )
        {
            throw null;
        }
    }

    partial public class Matcher
    {
        public Matcher() { }

        public Matcher(System.StringComparison comparisonType) { }

        public virtual Microsoft.Extensions.FileSystemGlobbing.Matcher AddExclude(string pattern)
        {
            throw null;
        }

        public virtual Microsoft.Extensions.FileSystemGlobbing.Matcher AddInclude(string pattern)
        {
            throw null;
        }

        public virtual Microsoft.Extensions.FileSystemGlobbing.PatternMatchingResult Execute(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directoryInfo
        )
        {
            throw null;
        }
    }

    partial public static class MatcherExtensions
    {
        public static void AddExcludePatterns(
            this Microsoft.Extensions.FileSystemGlobbing.Matcher matcher,
            params System.Collections.Generic.IEnumerable<string>[] excludePatternsGroups
        ) { }

        public static void AddIncludePatterns(
            this Microsoft.Extensions.FileSystemGlobbing.Matcher matcher,
            params System.Collections.Generic.IEnumerable<string>[] includePatternsGroups
        ) { }

        public static System.Collections.Generic.IEnumerable<string> GetResultsInFullPath(
            this Microsoft.Extensions.FileSystemGlobbing.Matcher matcher,
            string directoryPath
        )
        {
            throw null;
        }

        public static Microsoft.Extensions.FileSystemGlobbing.PatternMatchingResult Match(
            this Microsoft.Extensions.FileSystemGlobbing.Matcher matcher,
            System.Collections.Generic.IEnumerable<string>? files
        )
        {
            throw null;
        }

        public static Microsoft.Extensions.FileSystemGlobbing.PatternMatchingResult Match(
            this Microsoft.Extensions.FileSystemGlobbing.Matcher matcher,
            string file
        )
        {
            throw null;
        }

        public static Microsoft.Extensions.FileSystemGlobbing.PatternMatchingResult Match(
            this Microsoft.Extensions.FileSystemGlobbing.Matcher matcher,
            string rootDir,
            System.Collections.Generic.IEnumerable<string>? files
        )
        {
            throw null;
        }

        public static Microsoft.Extensions.FileSystemGlobbing.PatternMatchingResult Match(
            this Microsoft.Extensions.FileSystemGlobbing.Matcher matcher,
            string rootDir,
            string file
        )
        {
            throw null;
        }
    }

    partial public class PatternMatchingResult
    {
        public PatternMatchingResult(
            System.Collections.Generic.IEnumerable<Microsoft.Extensions.FileSystemGlobbing.FilePatternMatch> files
        ) { }

        public PatternMatchingResult(
            System.Collections.Generic.IEnumerable<Microsoft.Extensions.FileSystemGlobbing.FilePatternMatch> files,
            bool hasMatches
        ) { }

        public System.Collections.Generic.IEnumerable<Microsoft.Extensions.FileSystemGlobbing.FilePatternMatch> Files
        {
            get { throw null; }
            set { }
        }
        public bool HasMatches
        {
            get { throw null; }
        }
    }
}

namespace Microsoft.Extensions.FileSystemGlobbing.Abstractions
{
    partial public abstract class DirectoryInfoBase
        : Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileSystemInfoBase
    {
        protected DirectoryInfoBase() { }

        public abstract System.Collections.Generic.IEnumerable<Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileSystemInfoBase> EnumerateFileSystemInfos();
        public abstract Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? GetDirectory(
            string path
        );
        public abstract Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase? GetFile(
            string path
        );
    }

    partial public class DirectoryInfoWrapper
        : Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase
    {
        public DirectoryInfoWrapper(System.IO.DirectoryInfo directoryInfo) { }

        public override string FullName
        {
            get { throw null; }
        }
        public override string Name
        {
            get { throw null; }
        }
        public override Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? ParentDirectory
        {
            get { throw null; }
        }

        public override System.Collections.Generic.IEnumerable<Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileSystemInfoBase> EnumerateFileSystemInfos()
        {
            throw null;
        }

        public override Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? GetDirectory(
            string name
        )
        {
            throw null;
        }

        public override Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase GetFile(
            string name
        )
        {
            throw null;
        }
    }

    partial public abstract class FileInfoBase
        : Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileSystemInfoBase
    {
        protected FileInfoBase() { }
    }

    partial public class FileInfoWrapper
        : Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase
    {
        public FileInfoWrapper(System.IO.FileInfo fileInfo) { }

        public override string FullName
        {
            get { throw null; }
        }
        public override string Name
        {
            get { throw null; }
        }
        public override Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? ParentDirectory
        {
            get { throw null; }
        }
    }

    partial public abstract class FileSystemInfoBase
    {
        protected FileSystemInfoBase() { }

        public abstract string FullName { get; }
        public abstract string Name { get; }
        public abstract Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase? ParentDirectory { get; }
    }
}

namespace Microsoft.Extensions.FileSystemGlobbing.Internal
{
    partial public interface ILinearPattern
        : Microsoft.Extensions.FileSystemGlobbing.Internal.IPattern
    {
        System.Collections.Generic.IList<Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment> Segments { get; }
    }

    partial public interface IPathSegment
    {
        bool CanProduceStem { get; }
        bool Match(string value);
    }

    partial public interface IPattern
    {
        Microsoft.Extensions.FileSystemGlobbing.Internal.IPatternContext CreatePatternContextForExclude();
        Microsoft.Extensions.FileSystemGlobbing.Internal.IPatternContext CreatePatternContextForInclude();
    }

    partial public interface IPatternContext
    {
        void Declare(
            System.Action<
                Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment,
                bool
            > onDeclare
        );
        void PopDirectory();
        void PushDirectory(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        );
        bool Test(Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory);
        Microsoft.Extensions.FileSystemGlobbing.Internal.PatternTestResult Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase file
        );
    }

    partial public interface IRaggedPattern
        : Microsoft.Extensions.FileSystemGlobbing.Internal.IPattern
    {
        System.Collections.Generic.IList<System.Collections.Generic.IList<Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment>> Contains { get; }
        System.Collections.Generic.IList<Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment> EndsWith { get; }
        System.Collections.Generic.IList<Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment> Segments { get; }
        System.Collections.Generic.IList<Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment> StartsWith { get; }
    }

    partial public class MatcherContext
    {
        public MatcherContext(
            System.Collections.Generic.IEnumerable<Microsoft.Extensions.FileSystemGlobbing.Internal.IPattern> includePatterns,
            System.Collections.Generic.IEnumerable<Microsoft.Extensions.FileSystemGlobbing.Internal.IPattern> excludePatterns,
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directoryInfo,
            System.StringComparison comparison
        ) { }

        public Microsoft.Extensions.FileSystemGlobbing.PatternMatchingResult Execute()
        {
            throw null;
        }
    }

    partial public struct PatternTestResult
    {
        private object _dummy;
        private int _dummyPrimitive;
        public static readonly Microsoft.Extensions.FileSystemGlobbing.Internal.PatternTestResult Failed;
        public readonly bool IsSuccessful
        {
            get { throw null; }
        }
        public readonly string? Stem
        {
            get { throw null; }
        }

        public static Microsoft.Extensions.FileSystemGlobbing.Internal.PatternTestResult Success(
            string? stem
        )
        {
            throw null;
        }
    }
}

namespace Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments
{
    partial public class CurrentPathSegment
        : Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment
    {
        public CurrentPathSegment() { }

        public bool CanProduceStem
        {
            get { throw null; }
        }

        public bool Match(string value)
        {
            throw null;
        }
    }

    partial public class LiteralPathSegment
        : Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment
    {
        public LiteralPathSegment(string value, System.StringComparison comparisonType) { }

        public bool CanProduceStem
        {
            get { throw null; }
        }
        public string Value
        {
            get { throw null; }
        }

        public override bool Equals(
            [System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj
        )
        {
            throw null;
        }

        public override int GetHashCode()
        {
            throw null;
        }

        public bool Match(string value)
        {
            throw null;
        }
    }

    partial public class ParentPathSegment
        : Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment
    {
        public ParentPathSegment() { }

        public bool CanProduceStem
        {
            get { throw null; }
        }

        public bool Match(string value)
        {
            throw null;
        }
    }

    partial public class RecursiveWildcardSegment
        : Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment
    {
        public RecursiveWildcardSegment() { }

        public bool CanProduceStem
        {
            get { throw null; }
        }

        public bool Match(string value)
        {
            throw null;
        }
    }

    partial public class WildcardPathSegment
        : Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment
    {
        public static readonly Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments.WildcardPathSegment MatchAll;

        public WildcardPathSegment(
            string beginsWith,
            System.Collections.Generic.List<string> contains,
            string endsWith,
            System.StringComparison comparisonType
        ) { }

        public string BeginsWith
        {
            get { throw null; }
        }
        public bool CanProduceStem
        {
            get { throw null; }
        }
        public System.Collections.Generic.List<string> Contains
        {
            get { throw null; }
        }
        public string EndsWith
        {
            get { throw null; }
        }

        public bool Match(string value)
        {
            throw null;
        }
    }
}

namespace Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts
{
    partial public abstract class PatternContextLinear
        : Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts.PatternContext<Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts.PatternContextLinear.FrameData>
    {
        public PatternContextLinear(
            Microsoft.Extensions.FileSystemGlobbing.Internal.ILinearPattern pattern
        ) { }

        protected Microsoft.Extensions.FileSystemGlobbing.Internal.ILinearPattern Pattern
        {
            get { throw null; }
        }

        protected string CalculateStem(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase matchedFile
        )
        {
            throw null;
        }

        protected bool IsLastSegment()
        {
            throw null;
        }

        public override void PushDirectory(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        ) { }

        public override Microsoft.Extensions.FileSystemGlobbing.Internal.PatternTestResult Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase file
        )
        {
            throw null;
        }

        protected bool TestMatchingSegment(string value)
        {
            throw null;
        }

        partial public struct FrameData
        {
            private object _dummy;
            private int _dummyPrimitive;
            public bool InStem;
            public bool IsNotApplicable;
            public int SegmentIndex;
            public string? Stem
            {
                get { throw null; }
            }
            public System.Collections.Generic.IList<string> StemItems
            {
                get { throw null; }
            }
        }
    }

    partial public class PatternContextLinearExclude
        : Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts.PatternContextLinear
    {
        public PatternContextLinearExclude(
            Microsoft.Extensions.FileSystemGlobbing.Internal.ILinearPattern pattern
        )
            : base(default(Microsoft.Extensions.FileSystemGlobbing.Internal.ILinearPattern)) { }

        public override bool Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        )
        {
            throw null;
        }
    }

    partial public class PatternContextLinearInclude
        : Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts.PatternContextLinear
    {
        public PatternContextLinearInclude(
            Microsoft.Extensions.FileSystemGlobbing.Internal.ILinearPattern pattern
        )
            : base(default(Microsoft.Extensions.FileSystemGlobbing.Internal.ILinearPattern)) { }

        public override void Declare(
            System.Action<
                Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment,
                bool
            > onDeclare
        ) { }

        public override bool Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        )
        {
            throw null;
        }
    }

    partial public abstract class PatternContextRagged
        : Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts.PatternContext<Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts.PatternContextRagged.FrameData>
    {
        public PatternContextRagged(
            Microsoft.Extensions.FileSystemGlobbing.Internal.IRaggedPattern pattern
        ) { }

        protected Microsoft.Extensions.FileSystemGlobbing.Internal.IRaggedPattern Pattern
        {
            get { throw null; }
        }

        protected string CalculateStem(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase matchedFile
        )
        {
            throw null;
        }

        protected bool IsEndingGroup()
        {
            throw null;
        }

        protected bool IsStartingGroup()
        {
            throw null;
        }

        public override void PopDirectory() { }

        public sealed override void PushDirectory(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        ) { }

        public override Microsoft.Extensions.FileSystemGlobbing.Internal.PatternTestResult Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase file
        )
        {
            throw null;
        }

        protected bool TestMatchingGroup(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileSystemInfoBase value
        )
        {
            throw null;
        }

        protected bool TestMatchingSegment(string value)
        {
            throw null;
        }

        partial public struct FrameData
        {
            private object _dummy;
            private int _dummyPrimitive;
            public int BacktrackAvailable;
            public bool InStem;
            public bool IsNotApplicable;
            public System.Collections.Generic.IList<Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment> SegmentGroup;
            public int SegmentGroupIndex;
            public int SegmentIndex;
            public string? Stem
            {
                get { throw null; }
            }
            public System.Collections.Generic.IList<string> StemItems
            {
                get { throw null; }
            }
        }
    }

    partial public class PatternContextRaggedExclude
        : Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts.PatternContextRagged
    {
        public PatternContextRaggedExclude(
            Microsoft.Extensions.FileSystemGlobbing.Internal.IRaggedPattern pattern
        )
            : base(default(Microsoft.Extensions.FileSystemGlobbing.Internal.IRaggedPattern)) { }

        public override bool Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        )
        {
            throw null;
        }
    }

    partial public class PatternContextRaggedInclude
        : Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts.PatternContextRagged
    {
        public PatternContextRaggedInclude(
            Microsoft.Extensions.FileSystemGlobbing.Internal.IRaggedPattern pattern
        )
            : base(default(Microsoft.Extensions.FileSystemGlobbing.Internal.IRaggedPattern)) { }

        public override void Declare(
            System.Action<
                Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment,
                bool
            > onDeclare
        ) { }

        public override bool Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        )
        {
            throw null;
        }
    }

    partial public abstract class PatternContext<TFrame>
        : Microsoft.Extensions.FileSystemGlobbing.Internal.IPatternContext
        where TFrame : struct
    {
        protected TFrame Frame;

        protected PatternContext() { }

        public virtual void Declare(
            System.Action<
                Microsoft.Extensions.FileSystemGlobbing.Internal.IPathSegment,
                bool
            > declare
        ) { }

        protected bool IsStackEmpty()
        {
            throw null;
        }

        public virtual void PopDirectory() { }

        protected void PushDataFrame(TFrame frame) { }

        public abstract void PushDirectory(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        );
        public abstract bool Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoBase directory
        );
        public abstract Microsoft.Extensions.FileSystemGlobbing.Internal.PatternTestResult Test(
            Microsoft.Extensions.FileSystemGlobbing.Abstractions.FileInfoBase file
        );
    }
}

namespace Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns
{
    partial public class PatternBuilder
    {
        public PatternBuilder() { }

        public PatternBuilder(System.StringComparison comparisonType) { }

        public System.StringComparison ComparisonType
        {
            get { throw null; }
        }

        public Microsoft.Extensions.FileSystemGlobbing.Internal.IPattern Build(string pattern)
        {
            throw null;
        }
    }
}
