//Auto-edited to add globalization coverage by Globalizer, 6/3/2004 11:54:49 AM, written by RDawson
// ClassLib\Test\Utilities\Central.cool    GeneMi    1999/12/08

/**
    NOTICE:

    This master copy of this file is in tst\ClassLib\Test\utilities\.   It is copied to each subdirectory
that has tests, so that it will be auto imported.
*/


using System;
using System.Text;

public class Central_GlobIi
{
    public static String assemDebugMsg(
        String p_secondaryTypeAndText // Such as "POINTTOBREAK: find error E_38sn"
    )
    {
        StringBuilder strWrite = new StringBuilder(100);
        strWrite.Append("DEBUGINFO");
        strWrite.Append(Char.ToString((char)0x3)); // 3 ,Ctrl-C ,EEVT
        strWrite.Append(p_secondaryTypeAndText);
        return strWrite.ToString();
        // strWrite = strWrite.Append( new Char( (char)0x1A ).ToString() ); // 26 ,Ctrl-Z ,EMSG
    }

    public static String assemDebugMsg(
        String p_primaryType // DEBUGINFO, or perhaps FUTURE_CAUTION etc.
        ,
        String p_secondaryTypeAndText // Such as "POINTTOBREAK: find error E_38sn"
    )
    {
        StringBuilder sblWrite = new StringBuilder(99);

        sblWrite.Append(p_primaryType);
        sblWrite.Append(Char.ToString((char)0x3)); // 3 ,Ctrl-C ,EEVT
        sblWrite.Append(p_secondaryTypeAndText);
        // sblWrite.Append( new Char( (char)0x1A ).ToString() ); // 26 ,Ctrl-Z ,EMSG

        return sblWrite.ToString();
    }

    /////////////////////////////////////////////////////////////////////
    // DebugBreak
    /////////////////////////////////////////////////////////////////////

    public static void DebugBreak(String strTest, String strBreakpoint)
    {
        StringBuilder strDebug = new StringBuilder("POINTTOBREAK: find ", 100);
        strDebug.Append(strBreakpoint);
        strDebug.Append(" (");
        strDebug.Append(strTest);
        strDebug.Append(")");
        Console.Error.WriteLine(assemDebugMsg(strDebug.ToString()));
    }

    /////////////////////////////////////////////////////////////////////
    // DebugInfo
    /////////////////////////////////////////////////////////////////////

    public static void DebugInfo(String strTest, String strDebugInfo)
    {
        StringBuilder strDebug = new StringBuilder("EXTENDEDINFO: ", 100);
        strDebug.Append(strDebugInfo);
        strDebug.Append(" (");
        strDebug.Append(strTest);
        strDebug.Append(")");
        Console.Error.WriteLine(assemDebugMsg(strDebug.ToString()));
    }

    /////////////////////////////////////////////////////////////////////
    // DebugPath
    /////////////////////////////////////////////////////////////////////

    public static void DebugPath(String strTest, String strPath)
    {
        StringBuilder strDebug = new StringBuilder("PATHTOSOURCE: ", 100);
        strDebug.Append(strPath);
        strDebug.Append(strTest);
        Console.Error.WriteLine(assemDebugMsg(strDebug.ToString()));
    }

    /**  USAGE EXAMPLE...
        if (  // See vj/Math/Cb4064Log.java
                Central_GlobIi.GetPropSmallDivByBig( dubEquation ,dubExpected ) > +0.01
            ||  Central_GlobIi.GetPropSmallDivByBig( dubEquation ,dubExpected ) < -0.01
           )
    **/
    public static double GetPropSmallDivByBig(double p_do8a, double p_do8b)
    {
        double do8Big;
        double do8Small;

        double do8Ret;
        double do8w;
        ///


        if (p_do8a >= p_do8b)
        {
            do8Big = p_do8a;
            do8Small = p_do8b;
        }
        else
        {
            do8Big = p_do8b;
            do8Small = p_do8a;
        }

        if (do8Small == (double)0.0 && do8Big == (double)0.0)
            return (double)0.0;

        if (do8Small <= (double)0.0 && do8Big <= (double)0.0)
        {
            do8w = do8Big;
            do8Big = do8Small;
            do8Small = do8w;
        }

        if (
            (do8Big < (double)0.0 && do8Small > (double)0.0)
            || (do8Big > (double)0.0 && do8Small < (double)0.0)
        )
        {
            do8Ret = (Double.NaN); // white lie

            return do8Ret;
        }

        if (do8Big == (double)0.0)
        {
            do8Ret = (Double.NaN);

            return do8Ret;
        }

        do8Ret = ((double)(do8Small / do8Big));
        do8Ret = (double)1.0 - do8Ret;

        return do8Ret;
    } //EOMethod

    /**
    
        // Baloney below here, just avoid automation problems:
    
    
     public boolean runTest()
        {
        Console.Error.WriteLine( "Central.java  paSs  (nonTest, ignore)." );
        return true;
        }
    
     public static void main( String[] args )
        {
        boolean bResult = false; // Assume FAiL
        Central_GlobIi cbA = new Central_GlobIi();
        bResult = cbA.runTest();
        if ( bResult == true ) System.SetExitCode(0); else System.SetExitCode(1);
        }
    
    **/
}
// EOFile
