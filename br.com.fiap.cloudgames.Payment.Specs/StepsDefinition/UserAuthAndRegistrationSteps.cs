using Reqnroll;
using System;
using Xunit;

namespace br.com.fiap.cloudgames.Payment.Specs.StepsDefinition;

[Binding]
public class UserAuthAndRegistrationSteps
{
    private string? _email;
    private string? _password;
    private string? _firstName;
    private string? _lastName;
    private bool _existingUser;
    private bool _authSuccess;
    private bool _registrationSuccess;
    private string? _accessToken;
    private Guid? _userId;
    private string? _errorMessage;

    [Given("a user with email {string} and password {string} exists")]
    public void GivenAUserWithEmailAndPasswordExists(string email, string password)
    {
        _email = email;
        _password = password;
        _existingUser = true;
    }

    [Given("no user exists with email {string}")]
    public void GivenNoUserExistsWithEmail(string email)
    {
        _email = email;
        _existingUser = false;
    }

    [When("the user submits the login request with email {string} and password {string}")]
    public void WhenTheUserSubmitsTheLoginRequestWithEmailAndPassword(string email, string password)
    {
        if (_existingUser && email == _email && password == _password)
        {
            _authSuccess = true;
            _accessToken = "mocked-jwt-access-token";
        }
        else
        {
            _authSuccess = false;
        }
    }

    [When("the user submits the login request with email {string} and wrong password {string}")]
    public void WhenTheUserSubmitsTheLoginRequestWithEmailAndWrongPassword(string email, string wrongPassword)
    {
        _authSuccess = false;
    }

    [Then("the authentication should be successful")]
    public void ThenTheAuthenticationShouldBeSuccessful()
    {
        Assert.True(_authSuccess);
    }

    [Then("the response should contain an access token")]
    public void ThenTheResponseShouldContainAnAccessToken()
    {
        Assert.NotNull(_accessToken);
        Assert.NotEmpty(_accessToken);
    }

    [Then("the authentication should fail")]
    public void ThenTheAuthenticationShouldFail()
    {
        Assert.False(_authSuccess);
    }

    [Given("a user with first name {string}, last name {string}, email {string} and password {string}")]
    public void GivenAUserWithFirstNameLastNameEmailAndPassword(string firstName, string lastName, string email, string password)
    {
        _firstName = firstName;
        _lastName = lastName;
        _email = email;
        _password = password;
        _existingUser = false;
    }

    [Given("a user with email {string} already exists")]
    public void GivenAUserWithEmailAlreadyExists(string email)
    {
        _existingUser = true;
    }

    [When("the user submits the registration request")]
    public void WhenTheUserSubmitsTheRegistrationRequest()
    {
        if (_existingUser)
        {
            _registrationSuccess = false;
            _errorMessage = "Email already in use";
        }
        else
        {
            _registrationSuccess = true;
            _userId = Guid.NewGuid();
        }
    }

    [Then("the account should be created successfully")]
    public void ThenTheAccountShouldBeCreatedSuccessfully()
    {
        Assert.True(_registrationSuccess);
    }

    [Then("the response should contain the user id")]
    public void ThenTheResponseShouldContainTheUserId()
    {
        Assert.NotNull(_userId);
        Assert.NotEqual(Guid.Empty, _userId);
    }

    [Then("the registration should fail")]
    public void ThenTheRegistrationShouldFail()
    {
        Assert.False(_registrationSuccess);
    }

    [Then("an error {string} should be returned")]
    public void ThenAnErrorShouldBeReturned(string expectedError)
    {
        Assert.Equal(expectedError, _errorMessage);
    }
}
