package exelix11.sysdvr;

import android.content.Context;
import android.content.Intent;
import android.util.Log;

public class SystemHelper {
    public static boolean OpenURL(String Url) {
        try {
            Intent intent = new Intent(android.content.Intent.ACTION_VIEW, android.net.Uri.parse(Url));
            sysdvrActivity.instance.startActivity(intent);
            return true;
        }
        catch (Exception e) {
            sysdvrActivity.Log("OpenURL failed: " + e.toString());
            return false;
        }
    }

    public static String GetPackageName() {
        return sysdvrActivity.instance.getPackageName();
    }

    public static Context GetContextForClip() {
        return sysdvrActivity.instance.getApplicationContext();
    }
}
