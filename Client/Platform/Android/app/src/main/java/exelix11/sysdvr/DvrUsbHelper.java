package exelix11.sysdvr;

import android.annotation.SuppressLint;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbInterface;
import android.hardware.usb.UsbManager;
import android.os.Debug;
import android.util.Log;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Hashtable;

public class DvrUsbHelper {
    final static int ERROR = -1;
    static String lastErrorTxt = "No error";
    static ArrayList<String> devSnapshot = null;
    static Hashtable<Integer, UsbDeviceConnection> openDevices = new Hashtable<>();

    private static final String ACTION_USB_PERMISSION = "exelix11.sysdvr.USB_PERMISSION";

    static void Log(String message) {
        Log.i("SysDVRJava", message);
    }

    static void SetLastError(String error) {
        Log(error);
        lastErrorTxt = error;
    }

    public static void FreeCurrentSnapshot() {
        devSnapshot.clear();
        devSnapshot = null;
    }

    public static String GetLatError() {
        return lastErrorTxt;
    }

    static UsbManager GetUsbManager() {
        UsbManager res;
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.M) {
            res = (UsbManager) sysdvrActivity.instance.getSystemService(Context.USB_SERVICE);
        } else {
            SetLastError("SDK version not supported");
            return null;
        }

        if (res == null) {
            SetLastError("UsbManager was null");
            return null;
        }

        return res;
    }

    static boolean DevPermission(UsbManager mng, UsbDevice dev, boolean request) {
        if (!mng.hasPermission(dev)) {
            if (request) {
                Log.i("log", "Requesting perimssion");
                PendingIntent permissionIntent = PendingIntent.getBroadcast(sysdvrActivity.instance, 0, new Intent(ACTION_USB_PERMISSION), PendingIntent.FLAG_IMMUTABLE);
                mng.requestPermission(dev, permissionIntent);
            }
            return false;
        }
        return true;
    }

    @SuppressLint("NewApi")
    public static int SnapshotDevices(int vid, int pid) {
        Log("Searching for devices with vid: " + vid + " pid: " + pid);

        if (devSnapshot != null) {
            SetLastError("There is already an USB snapshot pending");
            return ERROR;
        }

        UsbManager usbManager = GetUsbManager();
        if (usbManager == null)
            return ERROR;

        devSnapshot = new ArrayList<>();
        HashMap<String, UsbDevice> deviceList = usbManager.getDeviceList();
        for (UsbDevice dev : deviceList.values()) {
            Log("found device " + dev);

            if (dev.getVendorId() != vid || dev.getProductId() != pid)
                continue;

            if (DevPermission(usbManager, dev, true))
                devSnapshot.add(dev.getSerialNumber());
        }

        return devSnapshot.size();
    }

    public static String GetSerialById(int id) {
        if (devSnapshot == null) {
            SetLastError("No snapshot available");
            return null;
        }

        if (devSnapshot.size() <= id || id < 0) {
            SetLastError("invalid id");
            return null;
        }

        return devSnapshot.get(id);
    }

    @SuppressLint("NewApi")
    public static int OpenBySerial(String serial) {
        Log("opening: " + serial);

        UsbManager mng = GetUsbManager();
        if (mng == null)
            return 0;

        for (UsbDevice d : mng.getDeviceList().values()) {
            if (!DevPermission(mng, d, false))
                continue;

            try {
                if (d.getSerialNumber().equals(serial)) {
                    UsbDeviceConnection conn = mng.openDevice(d);
                    UsbInterface iface = d.getInterface(0);
                    conn.claimInterface(iface, true);
                    int desc = conn.getFileDescriptor();
                    openDevices.put(desc, conn);
                    Log("Opened device with desc: " + desc);
                    return desc;
                }
            } catch (Exception ex) {
                SetLastError(ex.toString());
                return 0;
            }
        }

        SetLastError("Device not found");
        return 0;
    }

    public static void CloseDevice(int handle) {
        if (openDevices.containsKey(handle)) {
            UsbDeviceConnection conn = openDevices.get(handle);
            conn.close();
        }
    }
}
