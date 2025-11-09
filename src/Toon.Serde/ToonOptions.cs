namespace Toon.Serde
{
    public sealed class ToonOptions
    {
        // Indentation spaces for pretty printing; 0 => compact
        public int Indent { get; set; } = 0;
        // Stable key ordering (deterministic output)
        public bool StableKeys { get; set; } = true;
        // Safe strings: quote/escape ambiguous values
        public bool SafeStrings { get; set; } = true;
    }
}
