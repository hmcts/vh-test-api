Feature: Users
	In order to manage test users
	As an api service
	I want to be able to create, retrieve and delete users

Scenario: Get user details by username - OK
	Given I have a user
	And I have a valid get user details by username request
	When I send the request to the endpoint
	Then the response should have the status OK and success status True
	And the user details should be retrieved

Scenario: Get user details by username - Not Found
	Given I have a get user details by username request with a nonexistent username
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

#Scenario: Create new AD user - Created
#	Given I have a valid create AD user request
#	When I send the request to the endpoint
#	Then the response should have the status Created and success status True
#	And the details of the new AD user are retrieved