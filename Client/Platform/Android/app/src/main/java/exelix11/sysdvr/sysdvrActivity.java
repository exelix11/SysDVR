package exelix11.sysdvr;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.util.Log;

import org.libsdl.app.SDLActivity;

public class sysdvrActivity extends SDLActivity
{
    public static sysdvrActivity instance;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        Log("SysDVRActivity onCreate()");
		super.onCreate(savedInstanceState);
        instance = this;
        CheckPackageName();
		Log("SysDVRActivity created");
    }

    static boolean checkOnce = true;
    void CheckPackageName() {
        /*
        * I'm not really into the android world but apparently people reuploading existing apps with ads to the store is a thing.
        * i'm conflicted about fighting this as it would go against the open source nature of the project.
        * So for now i'll just have a slightly obfuscated check here, if you're just making a fork feel free to remove this.
        * My only condition is that you don't upload this to the play store.
        */
        if (!checkOnce)
            return;

        checkOnce = false;

        if (getPackageName().equals("exelix" + ((Integer)11).toString() + getString(R.string.hello_txt).charAt(3) + "sysdvr"))
            return;

        AlertDialog.Builder dlgAlert  = new AlertDialog.Builder(this);
        dlgAlert.setMessage("You're using a SyDVR-Client version that was not downloaded from the official GitHub repository. This is at your own risk.");
        dlgAlert.setTitle("Warning");
        dlgAlert.setPositiveButton("Dismiss",
                new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog,int id) {
                        dialog.cancel();
                    }
                });
        dlgAlert.setNeutralButton("Open GitHub page", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                SystemHelper.OpenURL("https://github.com/exelix11/SysDVR");
            }
        });
        dlgAlert.setCancelable(true);
        dlgAlert.create().show();
    }

    public static void Log(String message) {
        Log.i("SysDVRJava", message);
    }
}
