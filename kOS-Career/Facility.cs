using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgradeables;

namespace kOS.AddOns.kOSCareer
{
	[KOSNomenclature("Facility")]
	class Facility : kOS.Safe.Encapsulation.Structure
	{
		public SharedObjects Shared { get; set; }
		PSystemSetup.SpaceCenterFacility m_facility;

		public Facility(PSystemSetup.SpaceCenterFacility facility, SharedObjects shared)
		{
			Shared = shared;
			m_facility = facility;
			RegisterInitializer(InitializeSuffixes);
		}

		private void InitializeSuffixes()
		{
			AddSuffix("NAME", new Suffix<StringValue>(() => m_facility.facilityName));
			AddSuffix("DISPLAYNAME", new Suffix<StringValue>(() => m_facility.facilityDisplayName));
			AddSuffix("BODY", new Suffix<BodyTarget>(() => BodyTarget.CreateOrGetExisting(m_facility.hostBody, Shared)));
			AddSuffix("LEVEL", new Suffix<ScalarIntValue>(() => (int)m_facility.GetFacilityLevel()));
			AddSuffix("MAXLEVEL", new Suffix<ScalarIntValue>(MaxLevel));
			AddSuffix("UPGRADECOST", new Suffix<ScalarDoubleValue>(UpgradeCost));

			AddSuffix("UPGRADE", new NoArgsVoidSuffix(Upgrade));
		}

		ScenarioUpgradeableFacilities.ProtoUpgradeable ProtoFacility
		{
			get { return ScenarioUpgradeableFacilities.protoUpgradeables["SpaceCenter/" + m_facility.facilityName]; }
		}
		

		// does this include difficulty modifiers and strategy modifiers?
		private ScalarDoubleValue UpgradeCost()
		{
			var upgradeableFacility = ProtoFacility.facilityRefs[0];
			return upgradeableFacility.GetUpgradeCost();
		}

		private ScalarIntValue MaxLevel()
		{
			return ProtoFacility.GetLevelCount();
		}

		private void Upgrade()
		{
			var protoFacility= ProtoFacility;
			if (protoFacility.GetLevel() == protoFacility.GetLevelCount())
			{
				throw new KOSException("Facility is already max level");
			}

			var upgradeableFacility = protoFacility.facilityRefs[0];
			var upgradeCost = upgradeableFacility.GetUpgradeCost();
			if (!CurrencyModifierQuery.RunQuery(TransactionReasons.StructureConstruction, -upgradeCost, 0f, 0f).CanAfford())
			{
				throw new KOSException("Insufficient funds");
			}

			Funding.Instance.AddFunds(-upgradeCost, TransactionReasons.StructureConstruction);
			upgradeableFacility.SetLevel(upgradeableFacility.FacilityLevel + 1);
		}
	}
}
