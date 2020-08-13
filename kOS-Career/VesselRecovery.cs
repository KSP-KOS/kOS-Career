using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace kOS.AddOns.kOSCareer
{
	// To avoid some weird bugs, OnVesselRecoveryRequested must be called from LateUpdate, not Update where most props are executed.
	// To avoid constantly checking whether we need to recover every LateUpdate, we just create this simple behavior class which
	// updates once and deletes itself.
	class VesselRecovery : MonoBehaviour
	{
		public void LateUpdate()
		{
			// firing OnVesselRecoveryRequested does the same thing that recovering normally would
			// however that triggers a switch to the space center scene
			// onVesselRecovered is what ends up getting called in the space center scene
			GameEvents.OnVesselRecoveryRequested.Fire(VesselToRecover);
			//GameEvents.onVesselRecovered.Fire(VesselToRecover.protoVessel, false);
			//GameObject.DestroyImmediate(VesselToRecover.gameObject);
			//GameObject.Destroy(this.gameObject);
		}

		public global::Vessel VesselToRecover;

		public static void Recover(global::Vessel vessel)
		{
			var gameObject = GameObject.Instantiate(new GameObject("VesselRecovery", typeof(VesselRecovery)));
			gameObject.GetComponent<VesselRecovery>().VesselToRecover = vessel;
		}
	}
}
