using Koakuma.Game;
using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TGame.ECS;
using TGame.UI;
using UnityEngine;

public class LobbyUIMediator : UIMediator<LobbyUIView>
{
    private CharacterAttributeComponent attributeComponent;
    protected override void OnInit(LobbyUIView view)
    {
        base.OnInit(view);

    }

    protected override void OnHide()
    {
        this.view.startBtn.onClick.RemoveListener(OnClickEnterDungeon);
        base.OnHide();
    }


    protected override void OnShow(object arg)
    {
        base.OnShow(arg);

        ECSEntity ecsEntity = CharacterHelper.GetPlayerEntity();
        attributeComponent = ecsEntity?.GetComponent<CharacterAttributeComponent>();


        PlayerComponent playerCompoenent = GameManager.ECS.World.GetComponent<PlayerComponent>();

        if (playerCompoenent != null)
        {
            this.view.kkmaTxt.text = (playerCompoenent.gameInfo.kkma / 1000).ToString();
            this.view.coinTxt.text = (playerCompoenent.gameInfo.bod / 1000).ToString();

        }

        if (attributeComponent.valueMap.TryGetValue(CharacterAttributeType.Energy, out var energy))
        {
            this.view.energyTxt.text = energy.ToString();
        }


        ///btn ÊÂ¼þ
        this.view.startBtn.onClick.AddListener(OnClickEnterDungeon);

    }

    private void OnClickEnterDungeon()
    {
        GameManager.Message.Post(new MessageType.EnterDungeon()).Coroutine();
    }
}

