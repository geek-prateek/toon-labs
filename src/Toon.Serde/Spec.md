# TOON Spec (short)

This file describes canonical TOON encoding rules used by this reference library.

- Objects are encoded with `{` and `}` and key-value pairs as `key: value`.
- Arrays are encoded with `[` and `]`, elements separated by commas.
- Stable key ordering: keys sorted lexicographically by default.
- Strings are unquoted when safe (no whitespace or special chars). Otherwise JSON-style quoted strings are used.
- Numbers, booleans, and null map to their JSON textual forms.
- Indentation controlled by `ToonOptions.Indent`. 0 => compact (no indentation).
- This is a minimal spec for an encoder; a full parser (`FromToon`) must implement identical rules to guarantee round-trip equivalence.
