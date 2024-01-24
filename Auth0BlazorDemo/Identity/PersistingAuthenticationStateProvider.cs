using System.Diagnostics;
using Auth0BlazorDemo.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Auth0BlazorDemo.Identity;

public class PersistingAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private Task<AuthenticationState>? _authenticationStateTask;
    private readonly PersistentComponentState _state;
    private readonly PersistingComponentStateSubscription _subscription;
    private readonly IdentityOptions _options;

    public PersistingAuthenticationStateProvider(
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
        _state = persistentComponentState;
        AuthenticationStateChanged += OnAuthenticationStateChanged;
        _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    private async Task OnPersistingAsync()
    {
        if (_authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        var authenticationState = await _authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst(_options.ClaimsIdentity.UserIdClaimType)?.Value;
            var name = principal.FindFirst("name")?.Value;

            if (userId != null && name != null)
            {
                _state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserId = userId,
                    Name = name,
                });
            }
        }
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> authenticationStateTask)
    {
        _authenticationStateTask = authenticationStateTask;
    }

    public void Dispose()
    {
        _authenticationStateTask?.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        _subscription.Dispose();
    }
}