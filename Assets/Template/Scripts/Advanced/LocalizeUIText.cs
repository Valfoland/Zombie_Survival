using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalizeUIText : MonoBehaviour {
	Text text;
	TextMesh _textMesh;
	public bool isUpper;
	public bool capitalize;
	// Use this for initialization
	void Awake () {
		if(!text)
		{
			text = GetComponent<Text>();
			if(text)
			text.enabled = false;
		}


		if (!_textMesh)
		{
			_textMesh = GetComponent<TextMesh>();
		}

		if(!text && !_textMesh)
			Destroy(this);
	}

	void Start()
	{
		if(Localizator.isInited)
			MakeLocalized();
	}

	public void ForceUpdate()
	{
		MakeLocalized();
	}

	// Update is called once per frame
	void Update () {
		if(text || _textMesh)
			MakeLocalized();
	}

	void MakeLocalized()
	{
		if(Application.isPlaying)
		{
			if (Localizator.isInited)
			{
				if (!text)
				{
					text = GetComponent<Text> ();
					if (text)
					{
						if (isUpper)
							text.text = Localizator.Localize (text.text).ToUpper ();
						else
							text.text = Localizator.Localize (text.text);

						if (capitalize)
						{
							var s = Localizator.Localize (text.text);

							var builder = new System.Text.StringBuilder (s.ToLower ());

							builder [0] = char.ToUpper (s [0]);

							text.text = builder.ToString ();

						}
						text.enabled = true;
					}

				} 
				else
				{
					if (isUpper)
						text.text = Localizator.Localize (text.text).ToUpper ();
					else
						text.text = Localizator.Localize (text.text);

					if (capitalize)
					{
						var s = Localizator.Localize (text.text);

						var builder = new System.Text.StringBuilder (s.ToLower ());

						builder [0] = char.ToUpper (s [0]);

						text.text = builder.ToString ();

					}
					text.enabled = true;
				}

				if (!_textMesh)
				{
					_textMesh = GetComponent<TextMesh>();

					if (_textMesh)
						_textMesh.text = Localizator.Localize(_textMesh.text);
				}
				else
				{
					_textMesh.text = Localizator.Localize(_textMesh.text);
				}

			
				Destroy (this);
			} 

			////			else if(Application.isEditor)
			////			{
			////				if(!text)
			////					text = GetComponent<Text>();
			////				Localizator.ForceInit();
			////				if (isUpper)
			////					text.text = Localizator.Localize (text.text).ToUpper ();
			////				else
			////					text.text = Localizator.Localize (text.text);
			////
			////				if (capitalize)
			////				{
			////					var s = Localizator.Localize (text.text);
			////
			////					var builder = new System.Text.StringBuilder (s.ToLower());
			////
			////					builder [0] = char.ToUpper (s [0]);
			////
			////					text.text = builder.ToString ();
			////
			////				}
			//
			//				text.enabled = true;
			//				Destroy(this);
			//			}
		}
	}
}
