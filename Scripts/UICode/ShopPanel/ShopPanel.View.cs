using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//此脚本由工具自动生成 切勿手动修改

namespace IFrameWork
{
    public partial class ShopPanel : UIBase
    {
        private Button m_Button;
        private Text m_TextLegacy;
        private Toggle m_Toggle;
        private Transform m_Background;
        private Transform m_Checkmark;
        private Transform m_Label;
        private GameObject m_m_GameObject;
        private Button m_CloseButton;
        private Text m_CloseButtonm_TextLegacy;
        private Button m_OpenButton;
        private Text m_OpenButtonm_CloseButtonm_TextLegacy;
        private Slider m_m_Slider;
        private Slider m_m_Sliderm_Background;
        private Transform m_FillArea;
        private Transform m_Fill;
        private Transform m_HandleSlideArea;
        private Transform m_Handle;
        
        void OnViewInit()
        {
            m_Button = transform.Find("Button").GetComponent<Button>();
            m_TextLegacy = transform.Find("Button/TextLegacy").GetComponent<Text>();
            m_Toggle = transform.Find("Toggle").GetComponent<Toggle>();
            m_Background = transform.Find("Toggle/Background").GetComponent<Transform>();
            m_Checkmark = transform.Find("Toggle/Background/Checkmark").GetComponent<Transform>();
            m_Label = transform.Find("Toggle/Label").GetComponent<Transform>();
            m_m_GameObject = transform.Find("m_GameObject").GetComponent<GameObject>();
            m_CloseButton = transform.Find("CloseButton").GetComponent<Button>();
            m_CloseButtonm_TextLegacy = transform.Find("CloseButton/CloseButtonm_TextLegacy").GetComponent<Text>();
            m_OpenButton = transform.Find("OpenButton").GetComponent<Button>();
            m_OpenButtonm_CloseButtonm_TextLegacy = transform.Find("OpenButton/OpenButtonm_CloseButtonm_TextLegacy").GetComponent<Text>();
            m_m_Slider = transform.Find("m_Slider").GetComponent<Slider>();
            m_m_Sliderm_Background = transform.Find("m_Slider/m_Sliderm_Background").GetComponent<Slider>();
            m_FillArea = transform.Find("m_Slider/FillArea").GetComponent<Transform>();
            m_Fill = transform.Find("m_Slider/FillArea/Fill").GetComponent<Transform>();
            m_HandleSlideArea = transform.Find("m_Slider/HandleSlideArea").GetComponent<Transform>();
            m_Handle = transform.Find("m_Slider/HandleSlideArea/Handle").GetComponent<Transform>();
            

            //Binding
            m_Button.onClick.AddListener(Onm_ButtonClick);
            m_CloseButton.onClick.AddListener(Onm_CloseButtonClick);
            m_OpenButton.onClick.AddListener(Onm_OpenButtonClick);
            
        }
    }
}
