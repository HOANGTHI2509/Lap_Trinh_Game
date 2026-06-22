using Godot;
using System;

public partial class Skeleton : CharacterBody2D
{
    public const float Speed = 80f;
    public float Hp = 100f; // Máu của quái
    public float MaxHp = 100f;
    public int Level = 1; // Cấp độ của quái

    private AnimatedSprite2D anim;
    private Player player; // Đổi từ Node2D sang Player để gọi thẳng hàm
    private Vector2 direction = Vector2.Zero;
    private Random random = new Random();
    private double wanderTimer = 0;
    private double attackTimer = 0; // Thời gian chờ giữa 2 lần cắn
    
    // Biến cho việc Hồi sinh
    private bool isDead = false;
    private double deadTimer = 0;

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>("Sprite2D");
        player = GetNode<Player>("../Player"); // Tìm người chơi
        ChangeDirection(); 
        
        // Khởi tạo cấp độ và máu lúc mới vào game
        if (player != null)
        {
            Level = 1 + (player.Level - 1) / 2;
        }
        MaxHp = 100 + (Level - 1) * 30; // Mỗi cấp tăng 30 máu
        Hp = MaxHp;

        // Cập nhật thanh máu lúc mới vào game
        ProgressBar hpBar = GetNodeOrNull<ProgressBar>("ProgressBar");
        if (hpBar != null)
        {
            hpBar.MaxValue = MaxHp;
            hpBar.Value = Hp;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (player == null || isDead) return; // Nếu quái chết thì ngừng mọi hoạt động

        if (player.isDead) // Nếu người chơi đã chết, quái đứng im không làm gì
        {
            anim.Stop();
            return;
        }

        // Tính khoảng cách tới người chơi
        float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);

        if (distanceToPlayer <= 600f) // TẦM NHÌN: 600 pixel (Rộng gần nửa màn hình)
        {
            if (distanceToPlayer <= 110f) // TẦM ĐÁNH: 110 pixel
            {
                direction = Vector2.Zero; // Đứng lại để cắn
                
                // Đếm ngược thời gian cắn
                attackTimer -= delta;
                if (attackTimer <= 0)
                {
                    attackTimer = 1.0; // 1 giây cắn 1 lần
                    
                    // Sát thương của quái tăng theo cấp độ (mỗi cấp tăng 3 sát thương)
                    int minDmg = 6 + (Level - 1) * 3;
                    int maxDmg = 10 + (Level - 1) * 3;
                    float damage = random.Next(minDmg, maxDmg + 1); 
                    
                    player.TakeDamage(damage); // Gọi thẳng hàm C#
                }
            }
            else
            {
                // Nếu người chơi ở trong tầm nhìn -> Đuổi theo!
                direction = (player.GlobalPosition - GlobalPosition).Normalized();
                attackTimer = 0; // Reset lại để cắn liền khi tới nơi
            }
        }
        else
        {
            attackTimer = 0;
        }
        
        // Di chuyển
        Velocity = direction * Speed;

        // Lật mặt
        if (direction.X > 0) anim.FlipH = false;
        else if (direction.X < 0) anim.FlipH = true;

        if (direction != Vector2.Zero) anim.Play("walk");
        else anim.Stop();

        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        // XỬ LÝ HỒI SINH KHI CHẾT
        if (isDead)
        {
            deadTimer -= delta;
            if (deadTimer <= 0)
            {
                Respawn(); // Gọi hàm hồi sinh
            }
            return; // Thoát hàm, không làm gì thêm lúc đang chết
        }

        // Nếu ở xa người chơi quá 600 pixel thì mới đi loanh quanh
        if (player != null && GlobalPosition.DistanceTo(player.GlobalPosition) > 600f)
        {
            wanderTimer += delta;
            if (wanderTimer > 2)
            {
                wanderTimer = 0;
                ChangeDirection();
            }
        }
    }

    private void ChangeDirection()
    {
        int value = random.Next(4);
        switch (value)
        {
            case 0: direction = Vector2.Right; break;
            case 1: direction = Vector2.Left; break;
            case 2: direction = Vector2.Up; break;
            case 3: direction = Vector2.Down; break;
        }
    }

    // Hàm này sẽ được Player gọi khi chém trúng quái
    public void TakeDamage(float damage)
    {
        if (isDead) return; // Đã chết thì không nhận sát thương nữa

        Hp -= damage;
        GD.Print("Quái bị chém mất " + damage + " máu! Máu còn: " + Hp);

        // Cập nhật tụt thanh máu
        ProgressBar hpBar = GetNodeOrNull<ProgressBar>("ProgressBar");
        if (hpBar != null)
        {
            hpBar.Value = Hp;
        }

        if (Hp <= 0)
        {
            // Báo cho Player cộng kinh nghiệm (nếu Player còn sống)
            if (player != null && !player.isDead)
            {
                player.GainExp();
            }

            GD.Print("Quái đã gục ngã! Chờ 5 giây để hồi sinh...");
            isDead = true;
            Visible = false; // Tàng hình
            GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true); // Tắt va chạm (không cản đường nữa)
            deadTimer = 5.0; // Bắt đầu đếm 5 giây
        }
    }

    // Hàm hồi sinh quái vật
    private void Respawn()
    {
        isDead = false;
        
        // Cập nhật cấp độ quái dựa theo cấp người chơi
        // Cứ người chơi lên 2 cấp thì quái lên 1 cấp
        if (player != null)
        {
            Level = 1 + (player.Level - 1) / 2;
        }
        
        MaxHp = 100 + (Level - 1) * 30; // Mỗi cấp quái tăng 30 máu
        Hp = MaxHp; // Hồi đầy máu
        
        // Hiện hình trở lại và bật va chạm
        Visible = true;
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);
        
        // Cập nhật thanh máu UI
        ProgressBar hpBar = GetNodeOrNull<ProgressBar>("ProgressBar");
        if (hpBar != null) hpBar.Value = Hp;
        
        // Random vị trí xuất hiện mới, GIỚI HẠN LẠI VÙNG ĐẤT TRỐNG ĐỂ KHÔNG BỊ KẸT VÀO RỪNG
        float randX = random.Next(350, 750);
        float randY = random.Next(200, 450);
        GlobalPosition = new Vector2(randX, randY);
        
        ChangeDirection(); // Reset hướng đi
        GD.Print("Quái đã hồi sinh ở tọa độ mới! Cấp độ hiện tại: " + Level);
    }
}
