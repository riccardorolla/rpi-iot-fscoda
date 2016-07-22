/*
* Copyright (C) 2009 The Android Open Source Project
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;

namespace RoverSetup
{
	/// <summary>
	/// This is the main Activity that displays the current chat session.
	/// </summary>
	[Activity (Label = "@string/app_name",MainLauncher = true,
	           ConfigurationChanges=Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation)]
	public class RoverBluetooth : Activity
	{
		// Debugging
		private const string TAG = "RoverSetup";
		private const bool Debug = true;
	
		// Message types sent from the BluetoothChatService Handler
		// TODO: Make into Enums
		public const int MESSAGE_STATE_CHANGE = 1;
		public const int MESSAGE_READ = 2;
		public const int MESSAGE_WRITE = 3;
		public const int MESSAGE_DEVICE_NAME = 4;
		public const int MESSAGE_TOAST = 5;
	
		// Key names received from the BluetoothChatService Handler
		public const string DEVICE_NAME = "device_name";
		public const string TOAST = "toast";
	
		// Intent request codes
		// TODO: Make into Enums
		private const int REQUEST_CONNECT_DEVICE = 1;
		private const int REQUEST_ENABLE_BT = 2;
	
		// Layout Views
		protected TextView title;
		private ListView conversationView;
		private EditText outEditText;
		private Button sendButton;
	//	private Button wifiButton;
		// Name of the connected device
		protected string connectedDeviceName = null;
		// Array adapter for the conversation thread
		protected ArrayAdapter<string> conversationArrayAdapter;
		// String buffer for outgoing messages
		private StringBuffer outStringBuffer;
		// Local Bluetooth adapter
		private BluetoothAdapter bluetoothAdapter = null;
		// Member object for the chat services
		private BluetoothSerialService serialService = null;
		protected LinearLayout layout ;
		protected Dictionary<string,WifiNetwork>  wifinetworks = new Dictionary<string,WifiNetwork>();
		protected Context androidctx = null;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			if (Debug)
				Log.Error (TAG, "+++ ON CREATE +++");
			
			// Set up the window layout
			RequestWindowFeature (WindowFeatures.CustomTitle);
			SetContentView (Resource.Layout.main);
			Window.SetFeatureInt (WindowFeatures.CustomTitle, Resource.Layout.custom_title);
	
			// Set up the custom title
			title = FindViewById<TextView> (Resource.Id.title_left_text);
			title.SetText (Resource.String.app_name);
			title = FindViewById<TextView> (Resource.Id.title_right_text);
	
			// Get local Bluetooth adapter
			bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
	
			// If the adapter is null, then Bluetooth is not supported
			if (bluetoothAdapter == null) {
				Toast.MakeText (this, "Bluetooth is not available", ToastLength.Long).Show ();
				Finish ();
				return;
			}
		}
		
		protected override void OnStart ()
		{
			base.OnStart ();
			
			if (Debug)
				Log.Error (TAG, "++ ON START ++");
			
			// If BT is not on, request that it be enabled.
			// setupChat() will then be called during onActivityResult
			if (!bluetoothAdapter.IsEnabled) {
				Intent enableIntent = new Intent (BluetoothAdapter.ActionRequestEnable);
				StartActivityForResult (enableIntent, REQUEST_ENABLE_BT);
			// Otherwise, setup the chat session
			} else {
				if (serialService == null)
					SetupChat ();
			}
			var serverIntent = new Intent(this, typeof(DeviceListActivity));
			StartActivityForResult(serverIntent, REQUEST_CONNECT_DEVICE);
		
		}
		
		protected override void OnResume ()
		{
			base.OnResume ();
			
			// Performing this check in onResume() covers the case in which BT was
			// not enabled during onStart(), so we were paused to enable it...
			// onResume() will be called when ACTION_REQUEST_ENABLE activity returns.
			if (serialService != null) {
				// Only if the state is STATE_NONE, do we know that we haven't started already
				if (serialService.GetState () == BluetoothSerialService.STATE_NONE) {
					// Start the Bluetooth chat services
					serialService.Start ();
				}
			}
		}
		
		private void SetupChat ()
		{
			Log.Debug (TAG, "SetupChat()");
	
			// Initialize the array adapter for the conversation thread
			conversationArrayAdapter = new ArrayAdapter<string> (this, Resource.Layout.message);
			conversationView = FindViewById<ListView> (Resource.Id.@in);
			conversationView.Adapter = conversationArrayAdapter;
			 
			// Initialize the compose field with a listener for the return key
			outEditText = FindViewById<EditText> (Resource.Id.edit_text_out);
			// The action listener for the EditText widget, to listen for the return key
			outEditText.EditorAction += delegate(object sender, TextView.EditorActionEventArgs e) {
				// If the action is a key-up event on the return key, send the message
				if (e.ActionId == ImeAction.ImeNull && e.Event.Action == KeyEventActions.Up) {
					var message = new Java.Lang.String (((TextView) sender).Text);
					SendMessage (message);
				}	
			};
			androidctx = this;
			// Initialize the send button with a listener that for click events
			sendButton = FindViewById<Button> (Resource.Id.button_send);
			sendButton.Click += delegate(object sender, EventArgs e) {
				// Send a message using content of the edit text widget
				var view = FindViewById<TextView> (Resource.Id.edit_text_out);
				var message = new Java.Lang.String (view.Text);
				SendMessage (message);
			};
			// Initialize the wifi button with a listener that for click events
			// Initialize the send button with a listener that for click events

			// Initialize the BluetoothChatService to perform bluetooth connections
			serialService = new BluetoothSerialService (this, new MyHandler (this));
			
			// Initialize the buffer for outgoing messages
			outStringBuffer = new StringBuffer ("");

			layout = new LinearLayout (this);



		}
		
		protected override void OnPause ()
		{
			base.OnPause ();
			
			if (Debug)
				Log.Error (TAG, "- ON PAUSE -");
		}
		
		protected override void OnStop ()
		{
			base.OnStop ();
			
			if(Debug)
				Log.Error (TAG, "-- ON STOP --");
		}
		
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			
			// Stop the Bluetooth chat services
			if (serialService != null)
				serialService.Stop ();
			
			if (Debug)
				Log.Error (TAG, "--- ON DESTROY ---");
		}
		
		private void EnsureDiscoverable ()
		{
			if (Debug)
				Log.Debug (TAG, "ensure discoverable");
			
			if (bluetoothAdapter.ScanMode != ScanMode.ConnectableDiscoverable) {
				Intent discoverableIntent = new Intent (BluetoothAdapter.ActionRequestDiscoverable);
				discoverableIntent.PutExtra (BluetoothAdapter.ExtraDiscoverableDuration, 300);
				StartActivity (discoverableIntent);
			}
		}
		
		/// <summary>
		/// Sends a message.
		/// </summary>
		/// <param name='message'>
		/// A string of text to send.
		/// </param>
		private void SendMessage (Java.Lang.String message)
		{
			 string msg = message + "\n";
			// Check that we're actually connected before trying anything
			if (serialService.GetState () != BluetoothSerialService.STATE_CONNECTED) {
				Toast.MakeText (this, Resource.String.not_connected, ToastLength.Short).Show ();
				return;
			}
			
			// Check that there's actually something to send
			if (msg.Length > 0) {
				// Get the message bytes and tell the BluetoothChatService to write
				byte[] send =  System.Text.Encoding.ASCII.GetBytes (msg);
				serialService.Write (send);
	
				// Reset out string buffer to zero and clear the edit text field
				outStringBuffer.SetLength (0);
				outEditText.Text = string.Empty;
			}
		}
		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
		
		// The Handler that gets information back from the BluetoothChatService
		private class MyHandler : Handler
		{
			static bool wf = false;
			RoverBluetooth roverSetup;
			
			public MyHandler (RoverBluetooth chat)
			{
				roverSetup = chat;	
			}
			
			public override void HandleMessage (Message msg)
			{
				switch (msg.What) {
				case MESSAGE_STATE_CHANGE:
					if (Debug)
						Log.Info (TAG, "MESSAGE_STATE_CHANGE: " + msg.Arg1);
					switch (msg.Arg1) {
					case BluetoothSerialService.STATE_CONNECTED:
						roverSetup.title.SetText (Resource.String.title_connected_to);
						roverSetup.title.Append (roverSetup.connectedDeviceName);
						roverSetup.conversationArrayAdapter.Clear ();
						 
	
						break;
					case BluetoothSerialService.STATE_CONNECTING:
						roverSetup.title.SetText (Resource.String.title_connecting);
						break;
					case BluetoothSerialService.STATE_LISTEN:
					case BluetoothSerialService.STATE_NONE:
						roverSetup.title.SetText (Resource.String.title_not_connected);
						break;
					}
					break;
				case MESSAGE_WRITE:
					byte[] writeBuf = (byte[])msg.Obj;
					// construct a string from the buffer
					var writeMessage = new Java.Lang.String (writeBuf);
					roverSetup.conversationArrayAdapter.Add ("CMD: " + writeMessage);
					break;
				case MESSAGE_READ:
					byte[] readBuf = (byte[])msg.Obj;
					// construct a string from the valid bytes in the buffer
					var readMessage = new Java.Lang.String (readBuf, 0, msg.Arg1);
					if (readMessage.Contains("~") &&  !wf) {
						//Thread.Sleep (10000);
						//roverSetup.SendMessage (new Java.Lang.String ("\n\n\necho start"));
						Thread.Sleep (5000);
						roverSetup.SendMessage (new Java.Lang.String ("iwlist wlan0 scan | grep ESSID | sed 's\\[ ]*ESSID:\\wifi:\\g'"));
					}
						
					if (readMessage.StartsWith("wifi:")) {
						wf = true;
						string[] wifi = readMessage.Split(":");
						if (!roverSetup.wifinetworks.ContainsKey(wifi[1])) 
							roverSetup.wifinetworks.Add (wifi [1],null);
						Log.Debug (TAG, "wifi " + wifi[1]);

						var aLabel = new TextView(Application.Context);
						aLabel.Text =  wifi[1];
						var aText = new EditText(Application.Context);

						roverSetup.layout.AddView (aLabel);
						roverSetup.layout.AddView (aText);

						roverSetup.SetContentView(
					}
					roverSetup.conversationArrayAdapter.Add (readMessage);
					break;
				case MESSAGE_DEVICE_NAME:
					// save the connected device's name
					roverSetup.connectedDeviceName = msg.Data.GetString (DEVICE_NAME);
					Toast.MakeText (Application.Context, "Connected to " + roverSetup.connectedDeviceName, ToastLength.Short).Show ();
					break;
				case MESSAGE_TOAST:
					Toast.MakeText (Application.Context, msg.Data.GetString (TOAST), ToastLength.Short).Show ();
					break;
				}
			}
		}
		
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (Debug)
				Log.Debug (TAG, "onActivityResult " + resultCode);
			
			switch(requestCode)
			{
				case REQUEST_CONNECT_DEVICE:
					// When DeviceListActivity returns with a device to connect
					if( resultCode == Result.Ok)
					{
						// Get the device MAC address
						var address = data.Extras.GetString(DeviceListActivity.EXTRA_DEVICE_ADDRESS);
						// Get the BLuetoothDevice object
						BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(address);
						// Attempt to connect to the device
						serialService.Connect(device);
					}
					break;
				case REQUEST_ENABLE_BT:
					// When the request to enable Bluetooth returns
					if(resultCode == Result.Ok)
					{
						// Bluetooth is now enabled, so set up a chat session
						SetupChat();	
					}
					else
					{
						// User did not enable Bluetooth or an error occured
						Log.Debug(TAG, "BT not enabled");
						Toast.MakeText(this, Resource.String.bt_not_enabled_leaving, ToastLength.Short).Show();
						Finish();
					}
					break;
			}
		}
		
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			var inflater = MenuInflater;
			inflater.Inflate(Resource.Menu.option_menu, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) 
			{
				case Resource.Id.scan:
					// Launch the DeviceListActivity to see devices and do scan
					var serverIntent = new Intent(this, typeof(DeviceListActivity));
					StartActivityForResult(serverIntent, REQUEST_CONNECT_DEVICE);
					return true;
				case Resource.Id.discoverable:
					// Ensure this device is discoverable by others
					EnsureDiscoverable();
					return true;
			}
			return false;
		}
	}
}


