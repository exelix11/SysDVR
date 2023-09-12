package exelix11.sysdvr;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.provider.Settings;
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

    static boolean IsAndroid11OrAbove()
    {
        return android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.R;
    }

    public static int QueryPermissionInfo()
    {
        String legacyPermissionName = "android.permission.WRITE_EXTERNAL_STORAGE";

        try {
            boolean canWrite = false, canRequest = false;
            // Check if can write to external storage
            if (IsAndroid11OrAbove())
            {
                canWrite =  Environment.isExternalStorageManager();
            }
            else
            {
                int res = sysdvrActivity.instance.checkCallingOrSelfPermission(legacyPermissionName);
                canWrite = res == android.content.pm.PackageManager.PERMISSION_GRANTED;
            }

            // Check if can request permission
            if (!canWrite)
            {
                if (IsAndroid11OrAbove())
                    canRequest = true; // Always opens the settings
                else
                    canRequest = sysdvrActivity.instance.shouldShowRequestPermissionRationale(legacyPermissionName);
            }

            int result = 1;

            if (canWrite)
                result |= 2;

            if (canRequest)
                result |= 4;

            return result;
        }
        catch (Exception ex)
        {
            sysdvrActivity.Log("QueryPermissionInfo failed: " + ex.toString());
            return 0;
        }
    }

    public static void RequestFilePermission()
    {
        try {
            if (IsAndroid11OrAbove())
            {
                 Uri uri = Uri.parse("package:" + BuildConfig.APPLICATION_ID);
                 sysdvrActivity.instance.startActivity(new Intent(Settings.ACTION_MANAGE_APP_ALL_FILES_ACCESS_PERMISSION, uri));
            }
            else {
                String permission = "android.permission.WRITE_EXTERNAL_STORAGE";
                sysdvrActivity.instance.requestPermissions(new String[]{permission}, 1);
            }
        }
        catch (Exception ex)
        {
            sysdvrActivity.Log("RequestFilePermission failed: " + ex.toString());
        }
    }
}
