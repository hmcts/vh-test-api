Feature: Allocation Hearing
	In order to manage test users
	As an api service
	I want to be able to create hearings and allocate users

@VIH-6263
Scenario Outline: Allocate Users, Create Hearing and Create Conference
	Given I have the following list of users <userTypes>
	And I have an allocate users request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the user details for the <userTypes> allocations should be retrieved
	And the users should be allocated
	Given I have a create hearing request for the previously allocated users
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved
	Given I have a confirm hearing request
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the conference details should be retrieved
	Examples: 
	| userTypes                                                                 |
	| Judge, Individual, Representative, Individual, Representative, Case Admin |
	| Judge, Individual, Representative, Panel Member, Observer, Case Admin     |
