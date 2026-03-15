using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace UnityWarehouseSceneHDRP
{
    public class WarehouseUI : MonoBehaviour
    {
        [Header("팝업 패널")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private TMP_Text   titleText;       // 슬롯 위치 표시 (A-0-3)

        [Header("컨테이너 정보 표시 (컨테이너 있을 때)")]
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TMP_Text   infoIdText;
        [SerializeField] private TMP_Text   infoNameText;
        [SerializeField] private TMP_Text   infoWeightText;
        [SerializeField] private TMP_Text   infoDateText;

        [Header("입고 입력 필드 (비어있을 때)")]
        [SerializeField] private GameObject  inputPanel;
        [SerializeField] private TMP_InputField inputId;
        [SerializeField] private TMP_InputField inputName;
        [SerializeField] private TMP_InputField inputWeight;

        [Header("버튼")]
        [SerializeField] private Button btnIncoming;   // 입고
        [SerializeField] private Button btnMove;       // 이동
        [SerializeField] private Button btnOutgoing;   // 출고
        [SerializeField] private Button btnClose;      // 닫기

        private PalletSlot _currentSlot;
        private PalletSlot _moveSourceSlot;   // 이동 중일 때 출발 슬롯

        private void Awake()
        {
            popupPanel.SetActive(false);
            btnIncoming.onClick.AddListener(OnIncoming);
            btnMove.onClick.AddListener(OnMove);
            btnOutgoing.onClick.AddListener(OnOutgoing);
            btnClose.onClick.AddListener(ClosePopup);
        }

        // 팔레트 클릭 시 호출
        public void OpenPopup(PalletSlot slot)
        {
            // 이동 모드: 두 번째 클릭 = 목적지 선택
            if (_moveSourceSlot != null)
            {
                _moveSourceSlot.MoveContainerTo(slot);
                _moveSourceSlot = null;
                ClosePopup();
                return;
            }

            _currentSlot = slot;
            titleText.text = $"슬롯: {slot.shelf}-{slot.floor}-{slot.slot}";

            bool isEmpty = slot.IsEmpty;
            inputPanel.SetActive(isEmpty);
            infoPanel.SetActive(!isEmpty);
            btnIncoming.gameObject.SetActive(isEmpty);
            btnMove.gameObject.SetActive(!isEmpty);
            btnOutgoing.gameObject.SetActive(!isEmpty);

            if (!isEmpty)
            {
                infoIdText.text     = $"ID: {slot.container.containerId}";
                infoNameText.text   = $"물건: {slot.container.itemName}";
                infoWeightText.text = $"무게: {slot.container.weight} kg";
                infoDateText.text   = $"입고일: {slot.container.arrivalDate}";
            }
            else
            {
                inputId.text     = "";
                inputName.text   = "";
                inputWeight.text = "";
            }

            popupPanel.SetActive(true);
        }

        // 입고
        private void OnIncoming()
        {
            if (string.IsNullOrEmpty(inputId.text) || string.IsNullOrEmpty(inputName.text))
            {
                Debug.LogWarning("ID와 물건 이름을 입력해주세요.");
                return;
            }

            float weight = float.TryParse(inputWeight.text, out float w) ? w : 0f;

            var data = new ContainerData(
                inputId.text,
                inputName.text,
                weight,
                DateTime.Now.ToString("yyyy-MM-dd"),
                _currentSlot.shelf,
                _currentSlot.floor,
                _currentSlot.slot
            );

            _currentSlot.PlaceContainer(data);
            ClosePopup();
        }

        // 이동 (첫 번째 클릭 → 두 번째 클릭으로 목적지 선택)
        private void OnMove()
        {
            _moveSourceSlot = _currentSlot;
            titleText.text  = "이동할 목적지 팔레트를 클릭하세요";
            btnMove.gameObject.SetActive(false);
            btnOutgoing.gameObject.SetActive(false);
            btnClose.gameObject.SetActive(true);
        }

        // 출고
        private void OnOutgoing()
        {
            _currentSlot.RemoveContainer(deleteFromDB: true);
            ClosePopup();
        }

        private void ClosePopup()
        {
            _moveSourceSlot = null;
            popupPanel.SetActive(false);
        }
    }
}
