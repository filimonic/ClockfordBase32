# ClockfordBase32
C# Clockford's Base32 conversion library

### Using:
    using ClockfordBase32;

    byte[] b = CFBase32.FromBase32String("91JPRV3F5GG7EVVJDHJ0");
    String s = CFBase32.ToBase32String(System.Text.Encoding.UTF8.GetBytes("Hello, world"));

    String s = CFBase32.ToBase32String(Guid g);
    Guid g = CFBase32.GuidFromBase32String(String s);