using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RDTech;

namespace kOS.AddOns.kOSCareer
{
	[KOSNomenclature("TechNode")]
	class KOSTechNode : kOS.Safe.Encapsulation.Structure
	{
		ProtoTechNode m_node;

		public KOSTechNode(ProtoTechNode node)
		{
			m_node = node;
			RegisterInitializer(InitializeSuffixes);
		}

		private void InitializeSuffixes()
		{
			AddSuffix("TECHID", new Suffix<StringValue>(() => m_node.techID));
			AddSuffix("SCIENCECOST", new Suffix<ScalarIntValue>(() => m_node.scienceCost));
			AddSuffix("STATE", new Suffix<StringValue>(() => m_node.state.ToString()));
			AddSuffix("TITLE", new Suffix<StringValue>(() => ResearchAndDevelopment.GetTechnologyTitle(m_node.techID)));

			AddSuffix("RESEARCH", new NoArgsVoidSuffix(Research));
		}

		private void Research()
		{
			if (m_node.state == RDTech.State.Available)
			{
				throw new KOSException("Node is already purchased");
			}

			var host = ResearchAndDevelopment.Instance;
			if (host != null)
			{
				if (!CurrencyModifierQuery.RunQuery(TransactionReasons.RnDTechResearch, 0f, -m_node.scienceCost, 0f).CanAfford(delegate (Currency c)
				{
					throw new KOSException("Not enough science to research this node");
				}))
				{
					throw new KOSException("Not enough funds");
				}
				float scienceCostLimit = GameVariables.Instance.GetScienceCostLimit(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.ResearchAndDevelopment));
				if ((float)m_node.scienceCost > scienceCostLimit)
				{
					throw new KOSException("Node exceeds science cost limit");
				}
				host.AddScience(-m_node.scienceCost, TransactionReasons.RnDTechResearch);
			}
			ResearchAndDevelopment.Instance.UnlockProtoTechNode(m_node);
			m_node = ResearchAndDevelopment.Instance.GetTechState(m_node.techID);
		}
	}
}
