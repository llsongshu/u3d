using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IFrameWork
{
    public partial class ShopPanel : UIBase
    {
        public override void OnInit()
        {
            base.OnInit();
        
            OnViewInit();
        }

        private void Onm_ButtonClick()
        {
            Debug.Log("Onm_ButtonClick");
        }
        private void Onm_CloseButtonClick()
        {
            Debug.Log("Onm_CloseButtonClick");
        }
        private void Onm_OpenButtonClick()
        {
            Debug.Log("Onm_OpenButtonClick");
        }
        //Method
    }
}
