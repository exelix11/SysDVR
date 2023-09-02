package exelix11.sysdvr;

import android.os.Bundle;
import android.util.Log;

import org.libsdl.app.SDLActivity;

public class sysdvrActivity extends SDLActivity
{
    public static sysdvrActivity instance;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        instance = this;
    }

    public static void Log(String message) {
        Log.i("SysDVRJava", message);
    }
}
