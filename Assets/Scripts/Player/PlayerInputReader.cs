using UnityEngine;

// Этот скрипт собирает весь ввод игрока в одном месте.
// Поддерживает ПК и мобильное управление.
public class PlayerInputReader : MonoBehaviour
{
    [Header("Mode")]
    [SerializeField] private bool useMobileInput = false;
    // Если true — используем мобильный ввод.
    // Если false — используем клавиатуру и мышь.

    [Header("Mobile Stick References")]
    [SerializeField] private MobileJoystick leftJoystick;
    // Левый стик для движения.

    [SerializeField] private MobileJoystick rightJoystick;
    // Правый стик для обычного прицеливания и стрельбы.

    [Header("Mobile Button References")]
    [SerializeField] private MobileActionButton grenadeButton;
    // Кнопка гранаты.
    // Должна быть в режиме удержания.

    [SerializeField] private MobileActionButton claymoreButton;
    // Кнопка клеймора.
    // Тоже должна быть в режиме удержания.

    [SerializeField] private MobileActionButton reloadButton;
    // Кнопка перезарядки.

    [SerializeField] private MobileActionButton weaponSwitchButton;
    // Кнопка смены оружия.

    [SerializeField] private MobileActionButton pauseButton;
    // Кнопка паузы.

    [Header("Mobile Aim Settings")]
    [SerializeField] private float mobileAimThreshold = 0.2f;
    // Минимальное отклонение правого стика для прицеливания.

    [SerializeField] private float mobileShootThreshold = 0.65f;
    // Порог, после которого считается, что игрок стреляет.

    [Header("Ability Drag Settings")]
    [SerializeField] private float abilityDragThreshold = 30f;
    // Минимальная дистанция drag для гранаты и клеймора в пикселях.

    public Vector2 MoveInput { get; private set; }
    // Движение игрока.

    public Vector3 AimDirection { get; private set; }
    // Обычное направление прицеливания.

    public bool IsShooting { get; private set; }
    // Игрок сейчас стреляет.

    public bool ReloadPressed { get; private set; }
    // Перезарядка.

    public bool GrenadePressed { get; private set; }
    // Кнопка гранаты сейчас удерживается.

    public bool GrenadeReleased { get; private set; }
    // Кнопка гранаты была отпущена в этом кадре.

    public Vector3 GrenadeAimDirection { get; private set; }
    // Направление броска гранаты.

    public bool HasGrenadeAimDirection { get; private set; }
    // Есть ли валидное направление гранаты.

    public bool ClaymorePressed { get; private set; }
    // Кнопка клеймора сейчас удерживается.

    public bool ClaymoreReleased { get; private set; }
    // Кнопка клеймора была отпущена в этом кадре.

    public Vector3 ClaymoreAimDirection { get; private set; }
    // Направление установки клеймора.

    public bool HasClaymoreAimDirection { get; private set; }
    // Есть ли валидное направление клеймора.

    public bool PausePressed { get; private set; }
    // Пауза.

    public bool SwitchToRifle { get; private set; }
    // Переключение на автомат.

    public bool SwitchToShotgun { get; private set; }
    // Переключение на дробовик.

    public bool IsInputEnabled { get; private set; } = true;
    // Ввод вообще разрешён или нет.

    private Camera mainCamera;
    // Главная камера.

    private Vector3 lastValidAimDirection = Vector3.forward;
    // Последнее корректное направление обычного прицеливания.

    private Vector3 lastValidGrenadeAimDirection = Vector3.forward;
    // Последнее корректное направление гранаты.

    private Vector3 lastValidClaymoreAimDirection = Vector3.forward;
    // Последнее корректное направление клеймора.

    private bool mobileWeaponToggleState;
    // Внутренний переключатель для смены оружия на мобиле.

    private Vector2 grenadeDragStartScreenPosition;
    // Позиция, где игрок начал удерживать кнопку гранаты.

    private Vector2 claymoreDragStartScreenPosition;
    // Позиция, где игрок начал удерживать кнопку клеймора.

    private bool wasGrenadeHeldLastFrame;
    // Удерживалась ли кнопка гранаты в прошлом кадре.

    private bool wasClaymoreHeldLastFrame;
    // Удерживалась ли кнопка клеймора в прошлом кадре.

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!IsInputEnabled)
        {
            ClearInputState();
            return;
        }

        ResetOneFrameButtons();

        if (useMobileInput)
        {
            ReadMobileMovement();
            ReadMobileAimAndShoot();
            ReadMobileActions();
        }
        else
        {
            ReadKeyboardMovement();
            ReadMouseAim();
            ReadKeyboardActions();
        }
    }

    // Этот метод читает движение с клавиатуры.
    private void ReadKeyboardMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        MoveInput = new Vector2(horizontal, vertical).normalized;
    }

    // Этот метод считает направление мыши по плоскости земли.
    private void ReadMouseAim()
    {
        if (mainCamera == null)
        {
            AimDirection = Vector3.zero;
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);

            Vector3 direction = targetPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                AimDirection = Vector3.zero;
                return;
            }

            AimDirection = direction.normalized;
            lastValidAimDirection = AimDirection;
        }
        else
        {
            AimDirection = Vector3.zero;
        }
    }

    // Этот метод читает действия на ПК.
    private void ReadKeyboardActions()
    {
        IsShooting = Input.GetMouseButton(0);

        ReloadPressed = Input.GetKeyDown(KeyCode.R);

        GrenadePressed = Input.GetKey(KeyCode.G);
        GrenadeReleased = Input.GetKeyUp(KeyCode.G);

        if (AimDirection.sqrMagnitude > 0.001f)
        {
            GrenadeAimDirection = AimDirection;
            lastValidGrenadeAimDirection = GrenadeAimDirection;
            HasGrenadeAimDirection = true;

            ClaymoreAimDirection = AimDirection;
            lastValidClaymoreAimDirection = ClaymoreAimDirection;
            HasClaymoreAimDirection = true;
        }

        ClaymorePressed = Input.GetKey(KeyCode.C);
        ClaymoreReleased = Input.GetKeyUp(KeyCode.C);

        PausePressed = Input.GetKeyDown(KeyCode.Escape);

        SwitchToRifle = Input.GetKeyDown(KeyCode.Alpha1);
        SwitchToShotgun = Input.GetKeyDown(KeyCode.Alpha2);
    }

    // Этот метод читает движение с левого стика.
    private void ReadMobileMovement()
    {
        if (leftJoystick == null)
        {
            MoveInput = Vector2.zero;
            return;
        }

        MoveInput = Vector2.ClampMagnitude(leftJoystick.InputVector, 1f);
    }

    // Этот метод читает обычное прицеливание и стрельбу с правого стика.
    private void ReadMobileAimAndShoot()
    {
        if (rightJoystick == null)
        {
            AimDirection = lastValidAimDirection;
            IsShooting = false;
            return;
        }

        Vector2 stickInput = Vector2.ClampMagnitude(rightJoystick.InputVector, 1f);
        float stickStrength = stickInput.magnitude;

        if (stickStrength >= mobileAimThreshold)
        {
            AimDirection = new Vector3(stickInput.x, 0f, stickInput.y).normalized;
            lastValidAimDirection = AimDirection;
        }
        else
        {
            AimDirection = lastValidAimDirection;
        }

        // Пока игрок держит гранату или клеймор,
        // обычную стрельбу временно выключаем.
        if (GrenadePressed || ClaymorePressed)
        {
            IsShooting = false;
        }
        else
        {
            IsShooting = stickStrength >= mobileShootThreshold;
        }
    }

    // Этот метод читает мобильные кнопки.
    private void ReadMobileActions()
    {
        if (reloadButton != null)
        {
            ReloadPressed = reloadButton.GetButtonValue();
        }

        if (pauseButton != null)
        {
            PausePressed = pauseButton.GetButtonValue();
        }

        if (weaponSwitchButton != null && weaponSwitchButton.GetButtonValue())
        {
            mobileWeaponToggleState = !mobileWeaponToggleState;

            if (mobileWeaponToggleState)
            {
                SwitchToShotgun = true;
                SwitchToRifle = false;
            }
            else
            {
                SwitchToRifle = true;
                SwitchToShotgun = false;
            }
        }

        ReadMobileGrenadeAction();
        ReadMobileClaymoreAction();
    }

    // Этот метод отдельно обрабатывает удержание и drag-наведение гранаты на мобильном.
    private void ReadMobileGrenadeAction()
    {
        if (grenadeButton == null)
        {
            wasGrenadeHeldLastFrame = false;
            return;
        }

        GrenadePressed = grenadeButton.IsPressed;

        if (GrenadePressed && !wasGrenadeHeldLastFrame)
        {
            grenadeDragStartScreenPosition = Input.mousePosition;
        }

        if (GrenadePressed)
        {
            Vector2 currentScreenPosition = Input.mousePosition;
            Vector2 dragVector = currentScreenPosition - grenadeDragStartScreenPosition;

            if (dragVector.magnitude >= abilityDragThreshold)
            {
                Vector3 worldDirection = new Vector3(dragVector.x, 0f, dragVector.y).normalized;

                GrenadeAimDirection = worldDirection;
                lastValidGrenadeAimDirection = worldDirection;
                HasGrenadeAimDirection = true;
            }
            else
            {
                HasGrenadeAimDirection = false;
            }
        }

        if (!GrenadePressed && wasGrenadeHeldLastFrame)
        {
            GrenadeReleased = true;
            GrenadeAimDirection = lastValidGrenadeAimDirection;
            HasGrenadeAimDirection = true;
        }

        wasGrenadeHeldLastFrame = GrenadePressed;
    }

    // Этот метод отдельно обрабатывает удержание и drag-наведение клеймора на мобильном.
    private void ReadMobileClaymoreAction()
    {
        if (claymoreButton == null)
        {
            wasClaymoreHeldLastFrame = false;
            return;
        }

        ClaymorePressed = claymoreButton.IsPressed;

        if (ClaymorePressed && !wasClaymoreHeldLastFrame)
        {
            claymoreDragStartScreenPosition = Input.mousePosition;
        }

        if (ClaymorePressed)
        {
            Vector2 currentScreenPosition = Input.mousePosition;
            Vector2 dragVector = currentScreenPosition - claymoreDragStartScreenPosition;

            if (dragVector.magnitude >= abilityDragThreshold)
            {
                Vector3 worldDirection = new Vector3(dragVector.x, 0f, dragVector.y).normalized;

                ClaymoreAimDirection = worldDirection;
                lastValidClaymoreAimDirection = worldDirection;
                HasClaymoreAimDirection = true;
            }
            else
            {
                HasClaymoreAimDirection = false;
            }
        }

        if (!ClaymorePressed && wasClaymoreHeldLastFrame)
        {
            ClaymoreReleased = true;
            ClaymoreAimDirection = lastValidClaymoreAimDirection;
            HasClaymoreAimDirection = true;
        }

        wasClaymoreHeldLastFrame = ClaymorePressed;
    }

    // Этот метод сбрасывает одноразовые флаги в начале кадра.
    private void ResetOneFrameButtons()
    {
        ReloadPressed = false;

        GrenadeReleased = false;
        HasGrenadeAimDirection = false;

        ClaymoreReleased = false;
        HasClaymoreAimDirection = false;

        PausePressed = false;

        SwitchToRifle = false;
        SwitchToShotgun = false;
    }

    // Этот метод полностью очищает состояние ввода.
    private void ClearInputState()
    {
        MoveInput = Vector2.zero;
        AimDirection = lastValidAimDirection;

        IsShooting = false;

        ReloadPressed = false;

        GrenadePressed = false;
        GrenadeReleased = false;
        GrenadeAimDirection = Vector3.zero;
        HasGrenadeAimDirection = false;

        ClaymorePressed = false;
        ClaymoreReleased = false;
        ClaymoreAimDirection = Vector3.zero;
        HasClaymoreAimDirection = false;

        PausePressed = false;

        SwitchToRifle = false;
        SwitchToShotgun = false;

        wasGrenadeHeldLastFrame = false;
        wasClaymoreHeldLastFrame = false;
    }

    // Этот метод глобально включает или выключает ввод.
    public void SetInputEnabled(bool isEnabled)
    {
        IsInputEnabled = isEnabled;

        if (!IsInputEnabled)
        {
            ClearInputState();
        }
    }
}