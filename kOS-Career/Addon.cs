using Contracts;
using kOS;
using kOS.AddOns;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Suffixed;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace kOS.AddOns.kOSCareer
{
	[kOSAddon("CAREER")]
	[kOS.Safe.Utilities.KOSNomenclature("CareerAddon")]
	public class Addon : kOS.Suffixed.Addon
	{
		public Addon(SharedObjects shared) : base(shared)
		{
			InitializeSuffixes();
		}

		new public void InitializeSuffixes()
		{
			AddSuffix("CLOSEDIALOGS", new NoArgsVoidSuffix(CloseDialogs));

			AddSuffix("RECOVERVESSEL", new OneArgsSuffix<VesselTarget>(RecoverVessel, "Recovers the target vessel"));
			AddSuffix("ISRECOVERABLE", new OneArgsSuffix<BooleanValue, VesselTarget>(IsRecoverable, "Returns whether the target vessel is recoverable"));

			AddSuffix("FUNDS", new Suffix<ScalarDoubleValue>(() => Funding.Instance.Funds));
			AddSuffix("SCIENCE", new Suffix<ScalarDoubleValue>(() => ResearchAndDevelopment.Instance.Science));
			AddSuffix("REPUTATION", new Suffix<ScalarDoubleValue>(() => Reputation.Instance.reputation));

			AddSuffix("OFFEREDCONTRACTS", new NoArgsSuffix<ListValue<KOSContract>>(OfferedContracts, "Gets the list of offered contracts"));
			AddSuffix("ACTIVECONTRACTS", new NoArgsSuffix<ListValue<KOSContract>>(ActiveContracts, "Gets the list of active contracts"));
			AddSuffix("ALLCONTRACTS", new NoArgsSuffix<ListValue<KOSContract>>(AllContracts, "Gets the list of all contracts"));

			AddSuffix("TECHNODES", new NoArgsSuffix<ListValue<KOSTechNode>>(TechNodes));
			AddSuffix("NEXTTECHNODES", new NoArgsSuffix<ListValue<KOSTechNode>>(NextTechNodes));

			AddSuffix("FACILITIES", new NoArgsSuffix<ListValue<Facility>>(Facilities));
			AddSuffix("DebugFacilities", new NoArgsVoidSuffix(DebugFacilities));

			AddSuffix("ALLCREW", new NoArgsSuffix<ListValue<CrewMember>>(GetAllCrew));
			AddSuffix("ASSIGNEDCREW", new NoArgsSuffix<ListValue<CrewMember>>(() => ConvertCrewList(HighLogic.CurrentGame.CrewRoster.Crew.Where(pcm => pcm.rosterStatus == ProtoCrewMember.RosterStatus.Assigned))));
			AddSuffix("AVAILABLECREW", new NoArgsSuffix<ListValue<CrewMember>>(() => ConvertCrewList(HighLogic.CurrentGame.CrewRoster.Crew.Where(pcm => pcm.rosterStatus == ProtoCrewMember.RosterStatus.Available))));
			AddSuffix("APPLICANTS", new NoArgsSuffix<ListValue<CrewMember>>(GetApplicants));
			AddSuffix("HIREAPPLICANT", new OneArgsSuffix<BooleanValue, CrewMember>(HireCrew));
			AddSuffix("FIRECREW", new OneArgsSuffix<BooleanValue, CrewMember>(FireCrew));
			AddSuffix("CREWLIMIT", new NoArgsSuffix<ScalarIntValue>(GetCrewLimit));
			AddSuffix("HIRECOST", new NoArgsSuffix<ScalarDoubleValue>(GetHireCost));
			AddSuffix("CANHIRE", new NoArgsSuffix<BooleanValue>(CanHire));
		}

		private BooleanValue CanHire()
		{
			var activeCrews = HighLogic.CurrentGame.CrewRoster.GetActiveCrewCount();
			if (activeCrews < GetCrewLimit())
			{
				CurrencyModifierQuery currencyModifierQuery = CurrencyModifierQuery.RunQuery(TransactionReasons.CrewRecruited, 0f - GameVariables.Instance.GetRecruitHireCost(activeCrews), 0f, 0f);
				if ((HighLogic.CurrentGame != null && HighLogic.CurrentGame.Mode == Game.Modes.MISSION && HighLogic.CurrentGame.Parameters.CustomParams<Expansions.Missions.MissionParamsExtras>().astronautHiresAreFree) || currencyModifierQuery.CanAfford())
				{
					return true;
				}
			}
			return false;
		}

		private ScalarDoubleValue GetHireCost()
		{
			var activeCrews = HighLogic.CurrentGame.CrewRoster.GetActiveCrewCount();
			return GameVariables.Instance.GetRecruitHireCost(activeCrews);
		}

		private ListValue<CrewMember> GetAllCrew()
		{
			HighLogic.CurrentGame.CrewRoster.Update(Planetarium.GetUniversalTime());
			return ConvertCrewList(HighLogic.CurrentGame.CrewRoster.Crew);
		}

		private ListValue<CrewMember> GetApplicants()
		{
			HighLogic.CurrentGame.CrewRoster.Update(Planetarium.GetUniversalTime());
			return ConvertCrewList(HighLogic.CurrentGame.CrewRoster.Applicants);
		}

		private BooleanValue FireCrew(CrewMember crewMember)
		{
			try
			{
				ProtoCrewMember pcm = crewMember.crewMember;
				if (pcm.type == ProtoCrewMember.KerbalType.Crew && pcm.rosterStatus == ProtoCrewMember.RosterStatus.Available)
				{
					HighLogic.CurrentGame.CrewRoster.SackAvailable(pcm);
					return pcm.type == ProtoCrewMember.KerbalType.Applicant;
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}

			return false;
		}

		private ScalarIntValue GetCrewLimit()
		{
			return GameVariables.Instance.GetActiveCrewLimit(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.AstronautComplex));
		}

		private BooleanValue HireCrew(CrewMember crewMember)
		{
			try
			{
				if (CanHire())
				{
					ProtoCrewMember pcm = crewMember.crewMember;

					if (pcm.type == ProtoCrewMember.KerbalType.Applicant)
					{
						HighLogic.CurrentGame.CrewRoster.HireApplicant(pcm);
						return pcm.type == ProtoCrewMember.KerbalType.Crew;
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}

			return false;
		}

		

		private ListValue<CrewMember> ConvertCrewList(IEnumerable<ProtoCrewMember> crewList)
		{
			var result = new ListValue<CrewMember>();
			foreach (var pcm in crewList)
			{
				result.Add(new CrewMember(pcm, shared));
			}
			return result;
		}

		private void DebugFacilities()
		{
			var keys = string.Join(", ", ScenarioUpgradeableFacilities.protoUpgradeables.Keys);

			Debug.LogFormat("facility IDs: {0}", keys);
		}

		private ListValue<Facility> Facilities()
		{
			var result = new ListValue<Facility>();

			foreach (var f in PSystemSetup.Instance.SpaceCenterFacilities)
			{
				result.Add(new Facility(f, shared));
			}

			return result;
		}

		private ListValue<KOSTechNode> NextTechNodes()
		{
			var result = new ListValue<KOSTechNode>();

			AssetBase.RnDTechTree.ReLoad();
			AssetBase.RnDTechTree.GetNextUnavailableNodes().ForEach(node => result.Add(new KOSTechNode(node)));

			return result;
		}

		private ListValue<KOSTechNode> TechNodes()
		{
			var result = new ListValue<KOSTechNode>();

			Debug.LogFormat("spawned tech nodes");

			AssetBase.RnDTechTree.ReLoad();
			foreach (var node in AssetBase.RnDTechTree.GetTreeNodes())
			{
				Debug.LogFormat("creating node {0}", node.tech.techID);
				result.Add(new KOSTechNode(node.tech));
			}

			return result;
		}

		private void CloseDialogs()
		{
			KSP.UI.Dialogs.FlightResultsDialog.Close();

			var recoveryDialog = GameObject.FindObjectOfType<KSP.UI.Screens.MissionRecoveryDialog>();

			if (recoveryDialog != null)
			{
				GameObject.Destroy(recoveryDialog.gameObject);
			}

			var scienceDialog = KSP.UI.Screens.Flight.Dialogs.ExperimentsResultDialog.Instance;

			if (scienceDialog != null)
			{
				scienceDialog.currentPage.OnKeepData(scienceDialog.currentPage.pageData);
				scienceDialog.Dismiss();
			}
		}

		private ListValue<KOSContract> AllContracts()
		{
			ContractSystem.Instance.GenerateMandatoryContracts();

			ListValue<KOSContract> result = new ListValue<KOSContract>();
			foreach (var contract in ContractSystem.Instance.GetCurrentContracts<Contracts.Contract>())
			{
				result.Add(new KOSContract(contract));
			}

			return result;
		}
		
		private ListValue<KOSContract> ActiveContracts()
		{
			ListValue<KOSContract> result = new ListValue<KOSContract>();
			foreach (var contract in ContractSystem.Instance.GetCurrentActiveContracts<Contracts.Contract>())
			{
				result.Add(new KOSContract(contract));
			}

			return result;
		}

		private ListValue<KOSContract> OfferedContracts()
		{
			ContractSystem.Instance.GenerateMandatoryContracts();

			ListValue<KOSContract> result = new ListValue<KOSContract>();
			foreach (var contract in ContractSystem.Instance.GetCurrentContracts((Contracts.Contract c) => c.ContractState == Contracts.Contract.State.Offered))
			{
				result.Add(new KOSContract(contract));
			}

			return result;
		}

		private BooleanValue IsRecoverable(VesselTarget vessel)
		{
			return vessel != null && vessel.Vessel != null && vessel.Vessel.IsRecoverable;
		}

		private void RecoverVessel(VesselTarget vessel)
		{
			if (!vessel.Vessel.IsRecoverable)
			{
				throw new kOS.Safe.Exceptions.KOSException("Vessel is not recoverable");
			}
			VesselRecovery.Recover(vessel.Vessel);
		}

		public override BooleanValue Available()
		{
			return true;
		}
	}
}
