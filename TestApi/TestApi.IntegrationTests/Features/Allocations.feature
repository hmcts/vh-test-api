Feature: Allocations
	In order to manage test users
	As an api service
	I want to be able to create, retrieve and delete allocations

	Scenario Outline: Allocate user by user type and application - No Users Exist - OK
	Given I have a Allocate user by user type <UserType> and application request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the user details for the newly created <UserType> user during allocation should be retrieved
	And the user should be allocated
	Examples: 
	| UserType             |
	| CaseAdmin            |
	| VideoHearingsOfficer |
	| Judge                |
	| Individual           |
	| Representative       |
	| Observer             |
	| PanelMember          |

	Scenario Outline: Allocate user by user type and application - No Users Available - OK
	Given I have a <UserType> user with an allocation who is allocated
	And I have a Allocate user by user type <UserType> and application request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the user details for the newly created <UserType> user during allocation should be retrieved
	And the user should be allocated
	Examples: 
	| UserType             |
	| CaseAdmin            |
	| VideoHearingsOfficer |
	| Judge                |
	| Individual           |
	| Representative       |
	| Observer             |
	| PanelMember          |

	Scenario Outline: Allocate user by user type and application - Unallocated users available - OK
	Given I have a <UserType> user with an allocation who is unallocated
	And I have a Allocate user by user type <UserType> and application request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the user details should be retrieved
	And the user should be allocated
	Examples: 
	| UserType             |
	| CaseAdmin            |
	| VideoHearingsOfficer |
	| Judge                |
	| Individual           |
	| Representative       |
	| Observer             |
	| PanelMember          |

	Scenario: Unallocate allocated user by username - OK
	Given I have a user with an allocation who is allocated
	And I have another user with an allocation who is allocated
	And I have a valid unallocate users by username request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of user allocation details should be retrieved for the unallocated users

	Scenario: Unallocate unallocated user by username - OK
	Given I have a user with an allocation who is unallocated
	And I have another user with an allocation who is unallocated
	And I have a valid unallocate users by username request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And a list of user allocation details should be retrieved for the unallocated users

	Scenario: Unallocate user - NotFound
	Given I have a valid unallocate users by username request for a nonexistent user
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False