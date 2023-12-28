
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project3D.Lobbies
{
    public class Init : MonoBehaviour
    {
        // Start is called before the first frame update
        async void Start()
        {
            await UnityServices.InitializeAsync();

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                AuthenticationService.Instance.SignedIn += OnSignedIn;

                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn)
                {
                    string userName = AuthenticationService.Instance.PlayerId;
                }

                SceneManager.LoadSceneAsync("MainMenu");
            }
        }

        private void OnSignedIn()
        {
            Debug.Log($"playerID : {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"token : {AuthenticationService.Instance.AccessToken}");
        }
    }

}
