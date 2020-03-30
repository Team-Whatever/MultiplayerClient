using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ListExtension
{
    public static String ToArrayString<T>( this List<T> list ) where T : PlayerData
    {
        StringBuilder builder = new StringBuilder();
        foreach( var player in list )
        {
            builder.Append( player.ToString() + ";" );
        }
        return builder.ToString();
    }

    public static void Resize<T>( this List<T> list )
    {

    }
}

