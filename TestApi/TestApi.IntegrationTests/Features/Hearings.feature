@VIH-6262
Feature: Hearings
	In order to create test data
	As an api service
	I want to be able to create, retrieve and delete hearings and conferences

Scenario: Create a hearing - Created
	Given I have a create hearing request
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved

Scenario: Create a hearing in x mins time - Created
	Given I have a create hearing request in 10 minutes time
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved

Scenario: Create a hearing in x days time - Created
	Given I have a create hearing request in 10 days time
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved

Scenario: Create a hearing in a specified location - Created
	Given I have a create hearing request in location
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved

Scenario Outline: Create a hearing with list of partipant types - Created
	Given I have a create hearing request with a list of participants <participants>
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved
	Examples: 
	| participants                                           |
	| Individual, Representative, Individual, Representative |
	| Individual, Representative, Panel Member, Observer      |

Scenario: Create a hearing with a specified participant - Created
	Given I have a create hearing request with a list of participants Individual, Representative, Panel Member, Observer
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved
	Given I have a create hearing request with specific participants
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved

Scenario: Create a hearing with audio recording anabled - Created
	Given I have a create hearing request with audio recording enabled
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved

Scenario: Create a hearing with questionnaires anabled - Created
	Given I have a create hearing request with questionnaires enabled
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the hearing details should be retrieved

Scenario: Get Hearing - OK
	Given I have a hearing
	And I have a get hearing request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the hearing details should be retrieved

Scenario: Confirm Hearing - Created
	Given I have a hearing
	And I have a confirm hearing request
	When I send the request to the endpoint
	Then the response should have the status Created and success status True
	And the conference details should be retrieved

Scenario: Delete Hearing - No Content
	Given I have a hearing
	And I have a delete hearing request
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True