using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NpcFeature : BasicFeature {
	Animator animator;

	Vector3 spawnPos;
	Vector3 targetPos;
	FeatureInfoPanel p;
	bool Walking = false;
	float timer = 0;

	void Start()
	{
		animator = GetComponent<Animator> ();
		spawnPos = targetPos = transform.position;
		Vector2 random = new Vector2(spawnPos.x, spawnPos.z) + Random.insideUnitCircle * 3;
		targetPos = new Vector3 (random.x, 0, random.y);
	}
		

	public override void UpdateFunc ()
	{
		if (Vector3.Distance (transform.position, targetPos) > 0.01f) {
			if (!Walking) {
				animator.SetTrigger ("Walk");
				Walking = true;
			}
			transform.position += (targetPos - transform.position).normalized * 1.5f * Time.deltaTime;
			transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation ((targetPos - transform.position).normalized), Time.deltaTime * 1.5f);
			transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
		} else {
			if (Walking) {
				animator.SetTrigger ("StopWalk");
				Walking = false;
			}
			timer += Time.deltaTime;
			if (timer > 4.5f) {
				Vector2 random = new Vector2(spawnPos.x, spawnPos.z) + Random.insideUnitCircle * 3;
				targetPos = new Vector3 (random.x, 0, random.y);
				timer = 0;
			}
		}

		if (ClickedOnThis ()) {
			if (!selected && currentSelected == null) {
				FeatureInfoManager.instance.Clear ();
				currentSelected = this;
				StartCoroutine (resetSelected ());
                string[] mobs = extraData.Split(',');
				FeatureInfoManager.instance.ShowInfo (overHeadUIPos, FeatureName, mobs.Length.ToString(), "0");
			} else if (selected) {
				//PlayerPrefs.SetString("combatEnemyId", 
				PlayerPrefs.SetString ("monsterId", this.extraData);                
				Dictionary<string, object> data = new Dictionary<string, object> ();
				data.Add ("playerId", PlayerPrefs.GetString ("user"));
				data.Add ("featureId", this.UID);
				data.Add ("f_posX", this.x.ToStringEx());
				data.Add ("f_posY", this.y.ToStringEx());

				Bridge.POST (Bridge.url + "StartCombat", data,
					(r) => {
						Debug.Log ("[CombatStart] " + r);
						ServerResponse resp = new ServerResponse (r);
						if (resp.status != ServerResponse.ResultType.Error) {
							PlayerPrefs.SetString ("combatId", resp.GetIncomingDictionary () ["UID"].ToString ());
							PlayerPrefs.SetString ("combatFeatureId", this.UID);
							GameManager.LastCombatTarget = this;
							StartCoroutine (startCombat ());
						} else
                        {                            
                            Debug.Log("This combat is already over?");                            
                        }
					});
			}

			// Notify server of new combat start here

		}

	}	

	IEnumerator startCombat()
	{
		Camera.main.GetComponent<CameraController> ().LookPlayer = false;
		PlayerPrefs.SetFloat ("transformX", PlayerStats.instance.transform.position.x);
		PlayerPrefs.SetFloat ("transformY", PlayerStats.instance.transform.position.z);
		/*Vector3 target = transform.position;// + Vector3.up + Vector3.forward;
		float timer = 0;
		while (timer < 0.5f) {
			timer += Time.deltaTime;
			target = transform.position;
			Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, target, Time.deltaTime * 2.5f);
			Quaternion lookRot = Quaternion.LookRotation ((transform.position - Camera.main.transform.position).normalized);
			//Camera.main.transform.rotation = lookRot;
			yield return new WaitForEndOfFrame ();  
		}*/

		SceneManager.LoadScene ("Combat");
		yield return null;
	}
}
