using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace SmartMaker
{
	[AddComponentMenu("SmartMaker/Unity3D/WebCamManager")]
	public class WebCamManager : MonoBehaviour
	{
		[SerializeField]
		public List<string> deviceNames = new List<string>();
		public string deviceName;
		public int capWidth = 320;
		public int capHeight = 240;
		public int capFPS = 30;
		
		public Material material;
		public RawImage uiImage;
		
		public Text uiText;
		public RectTransform uiPanel;
		public GameObject uiItem;

		#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
		private WebCamTexture _webcam = null;
		#endif
		
		void Awake()
		{
		}
		
		// Use this for initialization
		void Start ()
		{
			
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}
		
		public void DeviceSearch()
		{
			deviceNames.Clear();

			#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			WebCamDevice[] devices = WebCamTexture.devices;
			foreach(WebCamDevice device in devices)
				deviceNames.Add(device.name);
			
			if(uiPanel != null && uiItem != null)
			{
				List<GameObject> items = new List<GameObject>();
				foreach(RectTransform rect in uiPanel)
				{
					if(rect.gameObject.Equals(uiItem) == false)
						items.Add(rect.gameObject);						
				}
				
				foreach(GameObject go in items)
					GameObject.DestroyImmediate(go);
				
				Text t = uiItem.GetComponent<Text>();
				if(t == null)
					t = uiItem.GetComponentInChildren<Text>();
				
				if(deviceNames.Count == 0)
				{
					if(t != null)
						t.text = "";
				}
				else
				{
					if(t != null)
						t.text = deviceNames[0];
					
					for(int i=1; i<deviceNames.Count; i++)
					{
						GameObject item = GameObject.Instantiate(uiItem);
						item.transform.SetParent(uiPanel.transform);
						t = item.GetComponent<Text>();
						if(t == null)
							t = item.GetComponentInChildren<Text>();
						if(t != null)
							t.text = deviceNames[i];
					}
				}
			}
			#endif
		}
		
		public void Play()
		{
			#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			if(_webcam == null)
				_webcam = new WebCamTexture();
			
			if(_webcam.isPlaying == true)
				_webcam.Stop();
			
			_webcam.deviceName = deviceName;
			_webcam.requestedWidth = capWidth;
			_webcam.requestedHeight = capHeight;
			_webcam.requestedFPS = capFPS;
			_webcam.Play();
			
			if(material != null)
				material.mainTexture = _webcam;
			
			if(uiImage != null)
				uiImage.texture = _webcam;
			#endif
		}
		
		public void Pause()
		{
			#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			if(_webcam == null)
				_webcam = new WebCamTexture();
			
			_webcam.Pause();
			#endif
		}
		
		public void Stop()
		{
			#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			if(_webcam == null)
				_webcam = new WebCamTexture();
			
			_webcam.Stop();
			#endif
		}
		
		public bool isPlaying
		{
			get
			{
				#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
				if(_webcam == null)
					return false;
				
				return _webcam.isPlaying;
				#else
				return false;
				#endif
			}
		}
		
		public int currentWidth
		{
			get
			{
				#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
				return _webcam.width;
				#else
				return capWidth;
				#endif
			}
		}
		
		public int currentHeight
		{
			get
			{
				#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
				return _webcam.height;
				#else
				return capHeight;
				#endif
			}
		}
		
		public void SelectDeviceName(Text text)
		{
			deviceName = text.text;
			if(uiText != null)
				uiText.text = deviceName;
		}
	}
}
