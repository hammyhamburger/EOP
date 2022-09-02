using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Cinemachine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }
    public TextMeshProUGUI playerNicknameTM;

    [Networked(OnChanged = nameof(OnNicknameChanged))]
    public NetworkString<_16> nickname { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Canvas playerNamePlate = GetComponentInChildren<Canvas>();
            playerNamePlate.enabled = false;

            RPC_SetNickname(PlayerPrefs.GetString("PlayerNickname"));

            Camera.main.gameObject.SetActive(false);

            Debug.Log("Spawned local player");
            transform.name = $"{nickname}_{Object.Id}";
        }
        else
        { 
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            CinemachineBrain cCamBrain = GetComponentInChildren<CinemachineBrain>();
            cCamBrain.enabled = false;

            CinemachineVirtualCamera cCam = GetComponentInChildren<CinemachineVirtualCamera>();
            cCam.enabled = false;
            
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            Debug.Log("Spawned remote player");
            transform.name = $"{nickname}_{Object.Id}";
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }

    static void OnNicknameChanged(Changed<NetworkPlayer> changed)
    {
        changed.Behaviour.OnNicknameChanged();
    }

    private void OnNicknameChanged()
    {
        playerNicknameTM.text = nickname.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickname(string nickname, RpcInfo info = default)
    {
        this.nickname = nickname;
    }
}
