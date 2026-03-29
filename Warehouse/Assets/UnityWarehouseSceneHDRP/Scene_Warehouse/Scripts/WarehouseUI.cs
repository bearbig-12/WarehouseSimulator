using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;

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
        [SerializeField] private TMP_Text   infoSizeText;  // 가로 × 세로 × 높이

        [Header("입고 입력 필드 (비어있을 때)")]
        [SerializeField] private GameObject  inputPanel;
        [SerializeField] private TMP_InputField inputId;
        [SerializeField] private TMP_InputField inputName;
        [SerializeField] private TMP_InputField inputWeight;
        [SerializeField] private TMP_InputField inputWidth;
        [SerializeField] private TMP_InputField inputDepth;
        [SerializeField] private TMP_InputField inputHeight;

        [Header("버튼")]
        [SerializeField] private Button btnIncoming;   // 입고
        [SerializeField] private Button btnMove;       // 이동
        [SerializeField] private Button btnOutgoing;   // 출고
        [SerializeField] private Button btnClose;      // 닫기

        [Header("입력 잠금 대상")]
        [SerializeField] private MonoBehaviour palletClickHandler;   // PalletClickHandler 컴포넌트

        private PalletSlot _currentSlot;
        private PalletSlot _moveSourceSlot;   // 이동 중일 때 출발 슬롯
        private bool       _blockPalletClick; // 팔렛 클릭 차단 여부

        private void Awake()
        {
            popupPanel.SetActive(false);
            btnIncoming.onClick.AddListener(OnIncoming);
            btnMove.onClick.AddListener(OnMove);
            btnOutgoing.onClick.AddListener(OnOutgoing);
            btnClose.onClick.AddListener(ClosePopup);

            // 숫자 전용 InputField 설정
            inputWeight.contentType = TMP_InputField.ContentType.DecimalNumber;
            inputWidth.contentType  = TMP_InputField.ContentType.DecimalNumber;
            inputDepth.contentType  = TMP_InputField.ContentType.DecimalNumber;
            inputHeight.contentType = TMP_InputField.ContentType.DecimalNumber;
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

            // 팔렛 클릭 차단 중이면 무시 (입력 필드 초기화 방지)
            if (_blockPalletClick) return;

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
                infoSizeText.text   = $"크기: {slot.container.width} × {slot.container.depth} × {slot.container.height} m";
            }
            else
            {
                inputId.text     = "";
                inputName.text   = "";
                inputWeight.text = "";
                inputWidth.text  = "";
                inputDepth.text  = "";
                inputHeight.text = "";
            }

            popupPanel.SetActive(true);
            _blockPalletClick = true;
            SetInputLock(true);
        }

        // 입고
        private void OnIncoming()
        {
            if (string.IsNullOrEmpty(inputId.text) || string.IsNullOrEmpty(inputName.text))
            {
                Debug.LogWarning("ID와 물건 이름을 입력해주세요.");
                return;
            }

            // ID 형식 체크 (CNT-000 ~ CNT-999)
            if (!Regex.IsMatch(inputId.text, @"^CNT-\d{3}$"))
            {
                Debug.LogWarning("ID 형식이 올바르지 않습니다. 예: CNT-001");
                inputId.text = "";
                return;
            }

            // 중복 ID 체크
            foreach (var s in FindObjectsByType<PalletSlot>(FindObjectsSortMode.None))
            {
                if (!s.IsEmpty && s.container.containerId == inputId.text)
                {
                    Debug.LogWarning($"이미 존재하는 컨테이너 ID입니다: {inputId.text}");
                    inputId.text = "";
                    return;
                }
            }

            float weight = float.TryParse(inputWeight.text, out float w)  ? w  : 0f;
            float width  = Mathf.Clamp(float.TryParse(inputWidth.text,  out float wx) ? wx : 1f, 0.1f, 5f);
            float depth  = Mathf.Clamp(float.TryParse(inputDepth.text,  out float d)  ? d  : 1f, 0.1f, 5f);
            float height = Mathf.Clamp(float.TryParse(inputHeight.text, out float h)  ? h  : 1f, 0.1f, 5f);

            var data = new ContainerData(
                inputId.text,
                inputName.text,
                weight,
                DateTime.Now.ToString("yyyy-MM-dd"),
                _currentSlot.shelf,
                _currentSlot.floor,
                _currentSlot.slot,
                width, depth, height
            );

            _currentSlot.PlaceContainer(data);
            ClosePopup();
        }

        // 이동 (첫 번째 클릭 → 두 번째 클릭으로 목적지 선택)
        private void OnMove()
        {
            _moveSourceSlot   = _currentSlot;
            _blockPalletClick = false; // 목적지 팔렛 클릭 허용
            titleText.text    = "이동할 목적지 팔레트를 클릭하세요";
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
            _moveSourceSlot   = null;
            _blockPalletClick = false;
            popupPanel.SetActive(false);
            SetInputLock(false);
        }

        private void SetInputLock(bool locked)
        {
            if (palletClickHandler != null) palletClickHandler.enabled = !locked;
        }
    }
}
