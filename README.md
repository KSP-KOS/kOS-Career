# kOS-Career
Addon for the Kerbal Space Program mod [kOS](https://ksp-kos.github.io/KOS/).  Adds functionality for interacting with career mode elements like contracts, building upgrades, and the tech tree.

## How to install

CKAN is recommended.  For a manual install, merge the contents of the GameData folder with your GameData folder.

## Table of Contents:
 
* [`CAREERADDON`](#careeraddon)
* [`CONTRACT`](#contract)
* [`CONTRACTPARAMETER`](#contractparameter)
* [`FACILITY`](#facility)
* [`TECHNODE`](#technode)

### `ADDONS:CAREER`

This is the entry point to the entire addon - see [below](#CAREERADDON) for the suffixes available.

### `CAREERADDON`

#### `:CLOSEDIALOGS()`

Closes UI elements such as the flight results, mission recovery, and science experiment results.

#### `:RECOVERVESSEL(vessel)`

Recovers the specified vessel.

#### `:ISRECOVERABLE(vessel)`

Returns a boolean indicating whether the specified vessel can be recovered.

#### `:FUNDS`

Gets the current available funds.

#### `:SCIENCE`

Gets the current avialable science points.

#### `:REPUTATION`

Gets the current reputation.

#### `:OFFEREDCONTRACTS()`

Returns a list of offered [`CONTRACT`](#Contract)s.  Note that this function has side effects: KSP does not generate contracts until you open the mission control window.  Similarly, calling this function will regenerate the list of offered contracts.

#### `:ACTIVECONTRACTS()`

Returns a list of active (accepted) [`CONTRACT`](#Contract)s.

#### `:ALLCONTRACTS()`

Returns a list of all [`CONTRACT`](#Contract)s.  Note that this function has side effects: KSP does not generate contracts until you open the mission control window.  Similarly, calling this function will regenerate the list of offered contracts.

#### `:TECHNODES()`

Returns a list of all [`TECHNODE`](#technode)s.

#### `:NEXTTECHNODES()`

Returns a list of [`TECHNODE`](#technode)s that have all prerequisites met and can be unlocked.

#### `:FACILITIES()`

Returns a list of the [`FACILITY`](#facility)s at KSC.

#### `:ALLCREW()`

Returns a list of all [`CrewMember`](https://ksp-kos.github.io/KOS/structures/vessels/crewmember.html)s.  Note that this function has side effects: it may respawn dead kerbals and refresh the applicant list.

#### `:ASSIGNEDCREW()`

Returns a list of [`CrewMember`](https://ksp-kos.github.io/KOS/structures/vessels/crewmember.html)s that are currently assigned (in a vessel).

#### `:AVAILABLECREW()`

Returns a list of [`CrewMember`](https://ksp-kos.github.io/KOS/structures/vessels/crewmember.html)s that are available for a new launch.

#### `:APPLICANTS()`

Returns a list of [`CrewMember`](https://ksp-kos.github.io/KOS/structures/vessels/crewmember.html)s that can be hired.  Note that this function has side effects: it may respawn dead kerbals and refresh the applicant list.

#### `:HIREAPPLICANT(CrewMember)`

Tries to hire an available [`CrewMember`](https://ksp-kos.github.io/KOS/structures/vessels/crewmember.html).  Returns a boolean indicating whether it succeeded.  Use `CANHIRE` to check if this will succeed.

#### `:FIRECREW(CrewMember)`

Tries to fire a [`CrewMember`](https://ksp-kos.github.io/KOS/structures/vessels/crewmember.html).  Returns a boolean indicating whether it succeeded (only available [`CrewMember`](https://ksp-kos.github.io/KOS/structures/vessels/crewmember.html)s can be fired).

#### `:CREWLIMIT`

Returns the current maximum number of crew.

#### `:HIRECOST`

Returns the current funds cost of hiring a new [`CrewMember`](https://ksp-kos.github.io/KOS/structures/vessels/crewmember.html).

#### `:CANHIRE`

Returns a boolean indicating whether you can hire a new kerbal.  This checks the hire cost, crew limit, and other game difficulty options.

### `CONTRACT`

A structure that represents a contract.

#### `:STATE`

Gets the current state of the contract as a string.  One of the following values:

 * `"Generated"`
 * `"Offered"`
 * `"OfferExpired"`
 * `"Declined"`
 * `"Cancelled"`
 * `"Active"`
 * `"Completed"`
 * `"DeadlineExpired"`
 * `"Failed"`
 * `"Withdrawn"`

#### `:AGENT`

Gets the name of the agency behind the contract.

#### `:FUNDSFAILURE`

Gets the amount of funds that will be lost if the contract is failed.

#### `:FUNDSADVANCE`

Gets the amount of funds awarded immediately when accepting the contract.

#### `:FUNDSCOMPLETION`

Gets the amount of funds awarded when completing the contract.

#### `:SCIENCECOMPLETION`

Gets the amount of science points awarded when completing the contract.

#### `:REPUTATIONCOMPLETION`

Gets the amount of reputation points awarded when completing the contract.

#### `:REPUTATIONFAILURE`

Gets the amount of reputation points that will be lost if the contract is failed.

#### `:TITLE`

Gets the title of the contract.

#### `:NOTES`

Gets any notes for the contract.

#### `:DESCRIPTION`

Gets the description for the contract

#### `:PARAMETERS`

Gets a list of [`CONTRACTPARAMETER`](#contractparameter) structures representing the individual objectives of the contract.

#### `:ACCEPT()`

Accepts the contract.  Throws an exception if the current state is not "Offered".

#### `:DECLINE()`

Declines an offered contract.  Throws an exception if the contract cannot be declined.

#### `:CANCEL()`

Cancels an active contract.  Throws an exception if the contract cannot be cancelled.

### `CONTRACTPARAMETER`

An individual objective for a contract.

#### `:STATE`

Gets the current state of the parameter as a string.  Possible values are:

* `"Incomplete"`
* `"Complete"`
* `"Failed"`

#### `:FUNDSFAILURE`

Gets the amount of funds that will be lost if the parameter is failed.

#### `:FUNDSCOMPLETION`

Gets the amount of funds awarded when completing the parameter.

#### `:SCIENCECOMPLETION`

Gets the amount of science points awarded when completing the parameter.

#### `:REPUTATIONCOMPLETION`

Gets the amount of reputation points awarded when completing the parameter.

#### `:REPUTATIONFAILURE`

Gets the amount of reputation points that will be lost if the paramater is failed.

#### `:TITLE`

Gets the title of the parameter.

#### `:NOTES`

Gets any notes for the parameter.

#### `:PARAMETERS`

Gets a list of [`CONTRACTPARAMETER`](#contractparameter) structures representing any sub-objectives of the parameter.

#### `:ID`

Returns the internal ID of the parameter.

#### `:CHEAT_SET_STATE(String)`

Sets the state of the parameter.  Valid values are `"Incomplete"`, `"Complete"`, and `"Failed"`.  Throws an exception if the value is invalid or if the parameter cannot be set to that state.

### `FACILITY`

A structure that represents one of the buildings at KSC.

#### `:NAME`

Gets the internal name of the facility.

#### `:DISPLAYNAME`

Gets the human-readable and localized name of the facility.

#### `:BODY`

Gets the body the facility is on.

#### `:LEVEL`

Gets the current level of the facility.

#### `:MAXLEVEL`

Gets the maximum level of the facility.

#### `:UPGRADECOST`

Gets the cost to upgrade this facility.

#### `:UPGRADE()`

Upgrades the facility.  Throws an exception if it cannot be done, for example if it is already max level or you do not have enough funds.

### `TECHNODE`

A structure that represents a node in the tech tree.

#### `:TECHID`

Gets the internal name of the tech node as a string.

#### `:SCIENCECOST`

Gets the cost in science points to research the node.

#### `:STATE`

Gets the current state of the node as a string.  Possible values are:

* `"Unavailable"` - the node has not been researched
* `"Available"` - the node has been researched

#### `:TITLE`

Gets the human-readable and localized name of the tech node.

#### `:RESEARCH()`

Researches the tech node.  Throws an exception if this is not possible, for example if it is already researched or if you do not have enough science points.

