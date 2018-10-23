using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour {
    public enum KeyStatus { Inactive, OnShow, Active};
    [Header("VFX")]
    //the vfx shown normally
    public GameObject InactiveBall;
    //the vfx shown after pressing E
    public GameObject HintBall;
    //the vfx shown after all matching
    public GameObject ActiveBall;
    //the vfxs shown after single matching
    public GameObject AlertBall;
    public GameObject ConfirmBall;
    public int DestroySceonds = 1;
    [Header("Light Up Settings")]
    public Light[] Lights;
    public float initEmissionScale = 1;
    public Material LightMaterial;
    public int OnLitFrames = 200;
	private GameObject character = null;

    private float[] lightRangeSteps, lightIntensitySteps;
    private float lightEmissionScaleStep;
    private int curOnLitFrame;
	private KeyMatcher matcher = new KeyMatcher();
    private KeyStatus status = KeyStatus.Inactive;

    // Use this for initialization
    void Start () {
        InactiveBall.GetComponent<ParticleSystem>().Play(true);
        ActiveBall.GetComponent<ParticleSystem>().Stop(true);
        lightRangeSteps = new float[Lights.Length];
        lightIntensitySteps = new float[Lights.Length];
        lightEmissionScaleStep = initEmissionScale / (float)OnLitFrames;
        //Debug.Log(lightEmissionScaleStep);
        LightMaterial.SetFloat("_EMISSION", 0);
        int id = 0;
        foreach (Light light in Lights)
        {
            lightRangeSteps[id] = light.GetComponent<Light>().range / (float)OnLitFrames;
            light.GetComponent<Light>().range = 0;
            lightIntensitySteps[id] = light.GetComponent<Light>().intensity / (float)OnLitFrames;
            light.GetComponent<Light>().intensity = 0;
            light.GetComponent<Light>().enabled = false;
            id++;
        }
        curOnLitFrame = OnLitFrames;
		matcher.maxMatchTime = this.GetComponentInChildren<Pattern>().pitchLength * 100;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if (curOnLitFrame < OnLitFrames)
		{
			curOnLitFrame++;
			int id = 0;
      LightMaterial.SetFloat("_EMISSION", LightMaterial.GetFloat("_EMISSION") + lightEmissionScaleStep);
			foreach (Light light in Lights)
			{
				//light.GetComponent<Light>().enabled = false;
				light.GetComponent<Light>().range += lightRangeSteps[id];
				light.GetComponent<Light>().intensity += lightIntensitySteps[id];
				id++;
			}
		}
		int pitchid = GetComponentInChildren<Pattern>().GetCurrentPitchId();
		if (character && matcher.active && pitchid > 0)
		{
			switch (matcher.TestMatch(character.GetComponent<Character>().soundValueAfterCalc, GetComponentInChildren<Pattern>().transform.localScale.x))
			{
				case KeyMatchStatus.FAILURE:
					//warn error
					matcher.ReSet();
					break;
				case KeyMatchStatus.SUCCESS:
					activate();
					character = null;
					break;
				case KeyMatchStatus.IGNORED:
				case KeyMatchStatus.INPROGRESS:
				default: break;
			}
		}
		else if (pitchid < 0) {
			matcher.ReSet();
		}
    }

    public string TriggerShow()
    {
        if(status == KeyStatus.Inactive)
        {
            InactiveBall.GetComponent<ParticleSystem>().Stop(true);
            this.GetComponentInChildren<Pattern>().StartHint();
            //walk on stage and disable moving
            status = KeyStatus.OnShow;
			character = GetComponent<InputListener>().character;
			Debug.Log("getCharacter: "+character);
			matcher.ReSet();
            return "Press E to stop. Match the spheres with your voice";
        }
        else if(status == KeyStatus.OnShow)
        {
            InactiveBall.GetComponent<ParticleSystem>().Play(true);
            this.GetComponentInChildren<Pattern>().StopHint();
            //walk off stage and enable moving
            status = KeyStatus.Inactive;
			character = null;
            return "Press E to dispaly.";
        }
        else
        {
            return "";
        }
    }

    public string activate()
    {
        this.GetComponentInChildren<Pattern>().StopHint();
        status = KeyStatus.Active;
        ActiveBall.GetComponent<ParticleSystem>().Play(true);
        curOnLitFrame = 0;
        foreach (Light light in Lights)
        {
            light.GetComponent<Light>().enabled = true;
        }
        return "Activated";
    }

    public int MatchSucceed()
    {
        GameObject tmp_ConfirmBall = Instantiate(ConfirmBall, this.transform);
        tmp_ConfirmBall.transform.position = HintBall.transform.position;
        tmp_ConfirmBall.transform.localScale = HintBall.transform.localScale;
        //HintBall.transform.localScale = new Vector3(0, 0, 0);
        //Debug.Log("haha");
        tmp_ConfirmBall.SetActive(true);
        StartCoroutine(WaitAndDestroy(tmp_ConfirmBall, DestroySceonds));
        return DestroySceonds;
    }

    public int MatchFail()
    {
        GameObject tmp_ConfirmBall = Instantiate(AlertBall, this.transform);
        tmp_ConfirmBall.transform.position = HintBall.transform.position;
        tmp_ConfirmBall.transform.localScale = HintBall.transform.localScale;
        //HintBall.transform.localScale = new Vector3(0, 0, 0);
        //Debug.Log("haha");
        tmp_ConfirmBall.SetActive(true);
        StartCoroutine(WaitAndDestroy(tmp_ConfirmBall, DestroySceonds, false));
        return DestroySceonds;
    }

    IEnumerator WaitAndDestroy(GameObject obj, int time, bool match = true)
    {
        yield return new WaitForSeconds(time);
        //HintBall.transform.localScale = obj.transform.localScale;
        if (!match)
            HintBall.transform.localScale = new Vector3(0, 0, 0);
        Destroy(obj);
    }
}
