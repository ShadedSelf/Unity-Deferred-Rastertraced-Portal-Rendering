using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class DOFF : MonoBehaviour {

	public bool doer = false;

	void Update ()
	{
		if (doer)
		{
			doer = false;
			SendMessage((IntPtr)HWND_BROADCAST, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)MONITOR_OFF);
			//Main();
		}
	}



	const int SC_MONITORPOWER = 0xF170;
	const int WM_SYSCOMMAND = 0x0112;

	const int MONITOR_ON = -1;
	const int MONITOR_OFF = 2;
	const int MONITOR_STANBY = 1;

	int HWND_BROADCAST = 0xffff;

	[DllImport("user32.dll")]
	static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

	const int SYSCOM = 0x0112;
	const int MONPOW = 0xF170;
	const int MON_ON = -1;
	const int MON_OFF = 2;
	const int MON_STANBY = 1;

	const int MONITOR_DEFAULTTOPRIMARY = 1;


	[DllImport("user32.dll")]
	static extern IntPtr SendMessage(IntPtr hwnd, uint msg, int wParam, int lParam);

	[DllImport("user32.dll", SetLastError = true)]
	static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

	[DllImport("user32.dll")]
	static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern bool GetDevicePowerState(IntPtr handle, out bool state);

	[DllImport("kernel32.dll")]
	static extern uint GetLastError();

	static void Main()
	{
		IntPtr handle = FindWindow(null, null);
		IntPtr monitorHandle = MonitorFromWindow(handle, MONITOR_DEFAULTTOPRIMARY);

		SendMessage(handle, SYSCOM, MONPOW, MON_OFF);

		bool isOn, result;
		result = GetDevicePowerState(monitorHandle, out isOn);
		uint err = GetLastError();


		if (result)
		{
			print("Monitor power state: " + isOn);
		}

		print("" + err);
	}

}


