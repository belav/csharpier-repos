using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

[RunInstaller(true)]
class MyInstall : Installer
{
    public MyInstall()
    {
        Committed += new InstallEventHandler(MyInstaller_Committed);
    }

    // Event handler for 'Committed' event.
    private void MyInstaller_Committed(object sender, InstallEventArgs e)
    {
        Console.WriteLine("");
        Console.WriteLine("Committed Event occured.");
        Console.WriteLine("");
    }

    public override void Install(IDictionary state)
    {
        Console.WriteLine("Installing");
    }
}

class X
{
    static void Main() { }
}
