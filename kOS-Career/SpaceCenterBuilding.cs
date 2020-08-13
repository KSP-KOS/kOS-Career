using kOS.Safe.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kOS.AddOns.kOSCareer
{
	[KOSNomenclature("SpaceCenterBuilding")]
	class KOSSpaceCenterBuilding : kOS.Safe.Encapsulation.Structure
	{
		global::SpaceCenterBuilding m_building;

		public KOSSpaceCenterBuilding(global::SpaceCenterBuilding building)
		{
			m_building = building;
		}
	}
}
