//I hope my code is readable enough that you don't need any comments.

using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;

public class colorOneTwoScript : MonoBehaviour 
{
	public KMBombInfo edgework;
	public KMAudio sound;
	public KMColorblindMode colorblind;

	public KMSelectable button1;
	public KMSelectable button2;
	
	private int leftLEDColor;
	private int rightLEDColor;
	
	public Renderer led1;
	public Renderer led2;
	public Material[] colors;
	public Material off;
	public int[] redArray;
	public int[] blueArray;
	public int[] greenArray;
	public int[] yellowArray;

	public TextMesh colorblindText1;
	public TextMesh colorblindText2;
	private bool colorblindActive;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool colorsPicked = false;
	private bool moduleSolved; 
	
	void Awake ()
	{
		moduleId = moduleIdCounter++;
		colorblindActive = colorblind.ColorblindModeActive;
		button1.OnInteract += delegate () { PressButton1(); return false; };
		button2.OnInteract += delegate () { PressButton2(); return false; };
		GetComponent<KMBombModule>().OnActivate += LightsOn;
	}
	
	void Start ()
	{
		if (colorsPicked == false)
		{
			PickLEDColor();
			colorsPicked = true;
		}
	}

	// Runs when the lights turn on in game
	void LightsOn()
	{
		if (!moduleSolved)
        {
			led1.material = colors[leftLEDColor];
			led2.material = colors[rightLEDColor];
			if (colorblindActive)
			{
				colorblindText1.text = colors[leftLEDColor].name;
				colorblindText2.text = colors[rightLEDColor].name;
			}
		}
	}

	void PickLEDColor()
	{
		leftLEDColor = UnityEngine.Random.Range(0,4);
		rightLEDColor = UnityEngine.Random.Range(0,4);
		Debug.LogFormat("[Color One Two #{0}] The left led's color is {1}.", moduleId, colors[leftLEDColor]);
		Debug.LogFormat("[Color One Two #{0}] The right led's color is {1}.", moduleId, colors[rightLEDColor]);
		if ((leftLEDColor == 0 && (redArray[rightLEDColor] == 1)) || (leftLEDColor == 1 && (blueArray[rightLEDColor] == 1)) || (leftLEDColor == 2 && (greenArray[rightLEDColor] == 1) || (leftLEDColor == 3 && (yellowArray[rightLEDColor] == 1))))
		{
			Debug.LogFormat("[Color One Two #{0}] You have to push button one.", moduleId);
		}
		else
		{
			Debug.LogFormat("[Color One Two #{0}] You have to push button two.", moduleId);
		}
	}
	
	void PressButton1()
	{
		Debug.LogFormat("[Color One Two #{0}] You pushed button one", moduleId);
		button1.AddInteractionPunch();
		sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, button1.transform);
		if (moduleSolved == false)
		{
			if ((leftLEDColor == 0 && (redArray[rightLEDColor] == 1)) || (leftLEDColor == 1 && (blueArray[rightLEDColor] == 1)) || (leftLEDColor == 2 && (greenArray[rightLEDColor] == 1) || (leftLEDColor == 3 && (yellowArray[rightLEDColor] == 1))))
			{
				moduleSolved = true;
				GetComponent<KMBombModule>().HandlePass();
				led1.material = off;
				led2.material = off;
				if (colorblindActive)
                {
					colorblindText1.text = "";
					colorblindText2.text = "";
				}
				Debug.LogFormat("[Color One Two #{0}] Correct! Module solved.", moduleId);
			}
			else
			{
				Debug.LogFormat("[Color One Two #{0}] Wrong! Module striked.", moduleId);
				GetComponent<KMBombModule>().HandleStrike();
			}
		}
	}
	
	void PressButton2()
	{
		Debug.LogFormat("[Color One Two #{0}] You pushed button two", moduleId);
		button2.AddInteractionPunch();
		sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, button2.transform);
		if (moduleSolved == false)
		{
			if ((leftLEDColor == 0 && (redArray[rightLEDColor] == 2)) || (leftLEDColor == 1 && (blueArray[rightLEDColor] == 2)) || (leftLEDColor == 2 && (greenArray[rightLEDColor] == 2) || (leftLEDColor == 3 && (yellowArray[rightLEDColor] == 2))))
			{
				moduleSolved = true;
				GetComponent<KMBombModule>().HandlePass();
				led1.material = off;
				led2.material = off;
				if (colorblindActive)
				{
					colorblindText1.text = "";
					colorblindText2.text = "";
				}
				Debug.LogFormat("[Color One Two #{0}] Correct! Module solved.", moduleId);
			}
			else
			{
				Debug.LogFormat("[Color One Two #{0}] Wrong! Module striked.", moduleId);
				GetComponent<KMBombModule>().HandleStrike();
			}
		}
	}

	//twitch plays
	#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} press # [Presses the button labelled with the number '#'] | !{0} colorblind [Toggles colorblind mode]";
	#pragma warning restore 414
	IEnumerator ProcessTwitchCommand(string command)
	{
		if (Regex.IsMatch(command, @"^\s*colorblind\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (colorblindActive)
			{
				colorblindActive = false;
				colorblindText1.text = "";
				colorblindText2.text = "";
			}
			else
			{
				colorblindActive = true;
				colorblindText1.text = colors[leftLEDColor].name;
				colorblindText2.text = colors[rightLEDColor].name;
			}
			yield break;
		}
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (parameters.Length > 2)
			{
				yield return "sendtochaterror Too many parameters!";
			}
			else if (parameters.Length == 2)
			{
				int temp = 0;
				if (!int.TryParse(parameters[1], out temp))
				{
					yield return "sendtochaterror The specified label " + parameters[1] + "' is not a number!";
					yield break;
				}
				if (temp < 1 || temp > 2)
				{
					yield return "sendtochaterror The specified label " + parameters[1] + "' is not in range 1-2!";
					yield break;
				}
				KMSelectable[] pressables = new KMSelectable[] { button1, button2 };
				pressables[temp - 1].OnInteract();
			}
			else if (parameters.Length == 1)
			{
				yield return "sendtochaterror Please specify the label of the button to press!";
			}
			yield break;
		}
	}

	IEnumerator TwitchHandleForcedSolve()
	{
		if ((leftLEDColor == 0 && (redArray[rightLEDColor] == 1)) || (leftLEDColor == 1 && (blueArray[rightLEDColor] == 1)) || (leftLEDColor == 2 && (greenArray[rightLEDColor] == 1) || (leftLEDColor == 3 && (yellowArray[rightLEDColor] == 1))))
			button1.OnInteract();
		else
			button2.OnInteract();
		yield return new WaitForSeconds(0.1f);
	}
}