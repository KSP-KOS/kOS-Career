using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kOS.AddOns.kOSCareer
{
	[KOSNomenclature("Contract")]
	class KOSContract : kOS.Safe.Encapsulation.Structure
	{
		Contracts.Contract m_contract;

		public KOSContract(Contracts.Contract contract)
		{
			m_contract = contract;
			RegisterInitializer(InitializeSuffixes);
		}

		private void InitializeSuffixes()
		{
			/*
			 * public double TimeDeadline;
			public double TimeExpiry;
			public virtual bool CanBeCancelled();
			public virtual bool CanBeDeclined();
			public virtual bool CanBeFailed();

			*/

			AddSuffix("STATE", new Suffix<StringValue>(() => m_contract.ContractState.ToString()));
			AddSuffix("AGENT", new Suffix<StringValue>(() => m_contract.Agent.Name));
			AddSuffix("FUNDSFAILURE", new Suffix<ScalarDoubleValue>(() => m_contract.FundsFailure));
			AddSuffix("FUNDSADVANCE", new Suffix<ScalarDoubleValue>(() => m_contract.FundsAdvance));
			AddSuffix("FUNDSCOMPLETION", new Suffix<ScalarDoubleValue>(() => m_contract.FundsCompletion));
			AddSuffix("SCIENCECOMPLETION", new Suffix<ScalarDoubleValue>(() => m_contract.ScienceCompletion));
			AddSuffix("REPUTATIONCOMPLETION", new Suffix<ScalarDoubleValue>(() => m_contract.ReputationCompletion));
			AddSuffix("REPUTATIONFAILURE", new Suffix<ScalarDoubleValue>(() => m_contract.ReputationFailure));
			AddSuffix("TITLE", new Suffix<StringValue>(() => m_contract.Title));
			AddSuffix("NOTES", new Suffix<StringValue>(() => m_contract.Notes));
			AddSuffix("DESCRIPTION", new Suffix<StringValue>(() => m_contract.Description));
			AddSuffix("PARAMETERS", new Suffix<ListValue<KOSContractParameter>>(GetParameters));

			AddSuffix("ACCEPT", new NoArgsVoidSuffix(Accept));
			AddSuffix("DECLINE", new NoArgsVoidSuffix(Decline));
			AddSuffix("CANCEL", new NoArgsVoidSuffix(Cancel));
		}

		private ListValue<KOSContractParameter> GetParameters()
		{
			var result = new ListValue<KOSContractParameter>();

			foreach(var parameter in m_contract.AllParameters)
			{
				result.Add(new KOSContractParameter(parameter));
			}

			return result;
		}

		private void Cancel()
		{
			if (!m_contract.CanBeCancelled()) throw new KOSException("Contract cannot be cancelled");
			m_contract.Cancel();
		}

		private void Decline()
		{
			if (!m_contract.CanBeDeclined()) throw new KOSException("Contract cannot be declined");
			m_contract.Decline();
		}

		private void Accept()
		{
			if (m_contract.ContractState != Contracts.Contract.State.Offered) throw new KOSException("Contract cannot be accepted");
			m_contract.Accept();
		}
	}

	[KOSNomenclature("ContractParameter")]
	class KOSContractParameter : kOS.Safe.Encapsulation.Structure
	{
		Contracts.ContractParameter m_parameter;

		public KOSContractParameter(Contracts.ContractParameter parameter)
		{
			m_parameter = parameter;
			RegisterInitializer(InitializeSuffixes);
		}

		private void InitializeSuffixes()
		{
			AddSuffix("STATE", new Suffix<StringValue>(() => m_parameter.State.ToString()));
			AddSuffix("FUNDSFAILURE", new Suffix<ScalarDoubleValue>(() => m_parameter.FundsFailure));
			AddSuffix("FUNDSCOMPLETION", new Suffix<ScalarDoubleValue>(() => m_parameter.FundsCompletion));
			AddSuffix("SCIENCECOMPLETION", new Suffix<ScalarDoubleValue>(() => m_parameter.ScienceCompletion));
			AddSuffix("REPUTATIONCOMPLETION", new Suffix<ScalarDoubleValue>(() => m_parameter.ReputationCompletion));
			AddSuffix("REPUTATIONFAILURE", new Suffix<ScalarDoubleValue>(() => m_parameter.ReputationFailure));
			AddSuffix("TITLE", new Suffix<StringValue>(() => m_parameter.Title));
			AddSuffix("NOTES", new Suffix<StringValue>(() => m_parameter.Notes));
			AddSuffix("PARAMETERS", new Suffix<ListValue<KOSContractParameter>>(GetParameters));
		}

		private ListValue<KOSContractParameter> GetParameters()
		{
			var result = new ListValue<KOSContractParameter>();

			foreach (var parameter in m_parameter.AllParameters)
			{
				result.Add(new KOSContractParameter(parameter));
			}

			return result;
		}
	}
}
