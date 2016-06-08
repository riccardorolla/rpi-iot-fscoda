package md52a44bcb96bb727ab29ff0c694f51adbd;


public class BluetoothSerialService_ConnectThread
	extends java.lang.Thread
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_run:()V:GetRunHandler\n" +
			"";
		mono.android.Runtime.register ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BluetoothSerialService_ConnectThread.class, __md_methods);
	}


	public BluetoothSerialService_ConnectThread () throws java.lang.Throwable
	{
		super ();
		if (getClass () == BluetoothSerialService_ConnectThread.class)
			mono.android.TypeManager.Activate ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public BluetoothSerialService_ConnectThread (java.lang.Runnable p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == BluetoothSerialService_ConnectThread.class)
			mono.android.TypeManager.Activate ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public BluetoothSerialService_ConnectThread (java.lang.Runnable p0, java.lang.String p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == BluetoothSerialService_ConnectThread.class)
			mono.android.TypeManager.Activate ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}


	public BluetoothSerialService_ConnectThread (java.lang.String p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == BluetoothSerialService_ConnectThread.class)
			mono.android.TypeManager.Activate ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0 });
	}


	public BluetoothSerialService_ConnectThread (java.lang.ThreadGroup p0, java.lang.Runnable p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == BluetoothSerialService_ConnectThread.class)
			mono.android.TypeManager.Activate ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.ThreadGroup, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public BluetoothSerialService_ConnectThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == BluetoothSerialService_ConnectThread.class)
			mono.android.TypeManager.Activate ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.ThreadGroup, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public BluetoothSerialService_ConnectThread (java.lang.ThreadGroup p0, java.lang.Runnable p1, java.lang.String p2, long p3) throws java.lang.Throwable
	{
		super (p0, p1, p2, p3);
		if (getClass () == BluetoothSerialService_ConnectThread.class)
			mono.android.TypeManager.Activate ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.ThreadGroup, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Java.Lang.IRunnable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int64, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public BluetoothSerialService_ConnectThread (java.lang.ThreadGroup p0, java.lang.String p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == BluetoothSerialService_ConnectThread.class)
			mono.android.TypeManager.Activate ("RoverSetup.BluetoothSerialService+ConnectThread, RoverSetup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Java.Lang.ThreadGroup, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.String, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1 });
	}


	public void run ()
	{
		n_run ();
	}

	private native void n_run ();

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
