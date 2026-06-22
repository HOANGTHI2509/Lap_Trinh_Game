using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public const float Speed = 300.0f;
    private AnimatedSprite2D _animatedSprite;
    
    // Biến để nhớ xem Dũng sĩ có đang bận chém quái không
    private bool _isAttacking = false;
    public bool isDead = false;

    public float Hp = 100f; // Máu của người chơi
    public float MaxHp = 100f;
    private Random random = new Random();

    // Hệ thống cấp độ
    public int Level = 1;
    public int Exp = 0; // Số quái đã giết ở cấp hiện tại
    private Label levelLabel;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        
        // Dặn game: Khi nào chạy xong hình ảnh chém, thì gọi hàm OnAnimationFinished để trả lại trạng thái bình thường
        _animatedSprite.AnimationFinished += OnAnimationFinished;

        // Cập nhật thanh máu lúc mới vào game
        ProgressBar hpBar = GetNodeOrNull<ProgressBar>("ProgressBar");
        if (hpBar != null)
        {
            hpBar.MaxValue = MaxHp;
            hpBar.Value = Hp;
        }

        // Tạo chữ hiển thị Level trên góc trái màn hình
        CanvasLayer canvas = new CanvasLayer();
        levelLabel = new Label();
        UpdateLevelUI();
        levelLabel.AddThemeFontSizeOverride("font_size", 30);
        levelLabel.AddThemeColorOverride("font_color", new Color(1, 1, 0)); // Màu vàng
        levelLabel.Position = new Vector2(20, 20); // Đặt ở góc trái
        canvas.AddChild(levelLabel);
        AddChild(canvas); // Gắn vào UI màn hình

        // --- MÀN HÌNH START GAME ---
        GetTree().Paused = true; // Tạm dừng game
        
        CanvasLayer startMenu = new CanvasLayer();
        startMenu.ProcessMode = Node.ProcessModeEnum.Always; // Để nút bấm vẫn hoạt động khi game đang dừng
        
        TextureRect startImg = new TextureRect();
        startImg.Texture = GD.Load<Texture2D>("res://menu_start.png");
        startImg.SetAnchorsPreset(Control.LayoutPreset.FullRect); 
        startImg.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        startImg.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
        
        // Tạo nút trong suốt đè lên chữ START GAME
        Button startBtn = new Button();
        startBtn.Flat = true; // Làm nút trong suốt
        startBtn.MouseDefaultCursorShape = Control.CursorShape.PointingHand; // Biến chuột thành hình bàn tay
        startBtn.Size = new Vector2(400, 80);
        startBtn.Position = new Vector2(1152 / 2 - 200, 390); // Tọa độ áng chừng chữ Start Game
        startBtn.Pressed += () => {
            startMenu.QueueFree(); // Tắt menu
            GetTree().Paused = false; // Chạy game
        };
        
        // Tạo nút trong suốt đè lên chữ QUIT
        Button quitBtn = new Button();
        quitBtn.Flat = true;
        quitBtn.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        quitBtn.Size = new Vector2(400, 80);
        quitBtn.Position = new Vector2(1152 / 2 - 200, 490);
        quitBtn.Pressed += () => {
            GetTree().Quit(); // Thoát game
        };

        startImg.AddChild(startBtn);
        startImg.AddChild(quitBtn);
        startMenu.AddChild(startImg);
        AddChild(startMenu);
    }

    public override void _Process(double delta)
    {
        // Kiểm tra xem nếu đã chết và người chơi bấm phím R thì chơi lại
        if (isDead && Input.IsPhysicalKeyPressed(Key.R))
        {
            GetTree().ReloadCurrentScene();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isDead) return; // Đã chết thì không thể di chuyển hay tấn công nữa

        // 1. NẾU ĐANG CHÉM -> Dừng lại, không cho chạy nữa
        if (_isAttacking)
        {
            Velocity = Vector2.Zero; 
            MoveAndSlide();
            return; // Thoát hàm luôn, không đọc code di chuyển bên dưới nữa
        }

        // 2. KIỂM TRA BẤM NÚT CHÉM (Phím Space)
        if (Input.IsActionJustPressed("attack"))
        {
            _isAttacking = true;            // Bật trạng thái đang chém
            _animatedSprite.Play("attack"); // Bật hình ảnh chém lửa

            // TÌM QUÁI VẬT VÀ TÍNH SÁT THƯƠNG
            Skeleton skeleton = GetNodeOrNull<Skeleton>("../Skeleton");
            if (skeleton != null && GlobalPosition.DistanceTo(skeleton.GlobalPosition) <= 130f) // Tầm chém: 130 pixel
            {
                // Tăng sát thương dựa theo cấp độ (mỗi cấp tăng 5 điểm)
                int minDmg = 10 + (Level - 1) * 5;
                int maxDmg = 20 + (Level - 1) * 5;
                float damage = random.Next(minDmg, maxDmg + 1); 
                
                // Tỷ lệ chí mạng: 20%
                if (random.Next(100) < 20)
                {
                    damage *= 2;
                    GD.Print(">>> CHÍ MẠNG! <<<");
                }
                
                // Trừ máu quái bằng cách gọi thẳng hàm C#
                skeleton.TakeDamage(damage);
            }

            return;
        }

        // 3. DI CHUYỂN BÌNH THƯỜNG
        Vector2 velocity = Velocity;
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (direction != Vector2.Zero)
        {
            velocity = direction * Speed;

            // Xử lý bật Animation 4 hướng
            if (direction.X > 0)
            {
                _animatedSprite.Play("walk_right");
            }
            else if (direction.X < 0)
            {
                _animatedSprite.Play("walk_left");
            }
            else if (direction.Y > 0)
            {
                _animatedSprite.Play("walk_down");
            }
            else if (direction.Y < 0)
            {
                _animatedSprite.Play("walk_up");
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
            _animatedSprite.Stop(); // Dừng chuyển động chân
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    // Hàm này tự động chạy khi một Animation bất kỳ vừa chạy xong 1 vòng
    private void OnAnimationFinished()
    {
        // Nếu cái animation vừa chạy xong là "attack", thì báo là đã chém xong, cho phép chạy tiếp
        if (_animatedSprite.Animation == "attack")
        {
            _isAttacking = false;
        }
    }

    // Hàm nhận sát thương khi bị quái cắn
    public void TakeDamage(float damage)
    {
        if (isDead) return; // Đã chết thì không trừ máu nữa

        Hp -= damage;
        GD.Print("Dũng sĩ bị trừ " + damage + " máu! Máu còn: " + Hp);

        // Cập nhật tụt thanh máu
        ProgressBar hpBar = GetNodeOrNull<ProgressBar>("ProgressBar");
        if (hpBar != null)
        {
            hpBar.Value = Hp;
        }

        if (Hp <= 0)
        {
            isDead = true;
            _animatedSprite.Stop(); // Dừng hiệu ứng chân bước
            GD.Print("GAME OVER! Dũng sĩ đã hy sinh.");
            
            // Thay vì dùng chữ, ta dùng hình ảnh Game Over của bạn
            CanvasLayer canvas = new CanvasLayer();
            canvas.ProcessMode = Node.ProcessModeEnum.Always; // Để nhận click khi dừng game

            TextureRect gameOverImg = new TextureRect();
            gameOverImg.Texture = GD.Load<Texture2D>("res://menu_gameover.png");
            
            // Ép ảnh phủ kín toàn bộ màn hình
            gameOverImg.SetAnchorsPreset(Control.LayoutPreset.FullRect); 
            gameOverImg.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            gameOverImg.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
            
            // Nút RETRY trong suốt
            Button retryBtn = new Button();
            retryBtn.Flat = true;
            retryBtn.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
            retryBtn.Size = new Vector2(400, 80);
            retryBtn.Position = new Vector2(1152 / 2 - 200, 420);
            retryBtn.Pressed += () => {
                GetTree().Paused = false;
                GetTree().ReloadCurrentScene();
            };

            // Nút EXIT trong suốt
            Button exitBtn = new Button();
            exitBtn.Flat = true;
            exitBtn.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
            exitBtn.Size = new Vector2(400, 80);
            exitBtn.Position = new Vector2(1152 / 2 - 200, 520);
            exitBtn.Pressed += () => {
                GetTree().Quit();
            };

            gameOverImg.AddChild(retryBtn);
            gameOverImg.AddChild(exitBtn);
            canvas.AddChild(gameOverImg);
            GetParent().AddChild(canvas);
            
            GetTree().Paused = true; // Dừng mọi hoạt động của game
        }
    }

    // Hàm nhận kinh nghiệm (gọi khi giết quái)
    public void GainExp()
    {
        if (Level >= 10) return; // Đạt cấp tối đa thì không lên nữa

        Exp++; // Tăng 1 điểm (1 con quái)
        
        // Kiểm tra xem đã đủ quái để lên cấp chưa
        if (Exp >= Level) 
        {
            Exp = 0; // Reset lại số quái
            Level++; // Tăng cấp
            
            // Phần thưởng lên cấp:
            MaxHp += 20; // Tăng máu tối đa
            Hp = MaxHp;  // Hồi đầy máu
            
            // Cập nhật lại thanh máu
            ProgressBar hpBar = GetNodeOrNull<ProgressBar>("ProgressBar");
            if (hpBar != null)
            {
                hpBar.MaxValue = MaxHp;
                hpBar.Value = Hp;
            }

            GD.Print(">>> CHÚC MỪNG BẠN ĐÃ LÊN CẤP " + Level + " <<<");
        }
        UpdateLevelUI();
    }

    // Cập nhật giao diện chữ góc trái
    private void UpdateLevelUI()
    {
        if (levelLabel != null)
        {
            if (Level >= 10)
                levelLabel.Text = "Cấp: 10 (Tối đa)";
            else
                levelLabel.Text = "Cấp: " + Level + " | Giết quái: " + Exp + "/" + Level;
        }
    }
}