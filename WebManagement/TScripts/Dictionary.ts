class TSDictionary<Tkey, TValue> {
    private keys: Tkey[] = [];
    private values: TValue[] = [];
    public constructor(Keys: Tkey[], Values: TValue[])
    {
        this.keys = Keys;
        this.values = Values;
    }
    Add(key: Tkey, value: TValue): void {
        this.keys.push(key);
        this.values.push(value);
    }
    Remove(key: Tkey): void {
        let index = this.keys.indexOf(key, 0);
        this.keys.splice(index, 1);
        this.values.splice(index, 1);
    }

    GetValue(key: Tkey): TValue {
        var index = this.keys.indexOf(key, 0);
        if (index != -1) {
            return this.values[index];
        }
        return null;
    }
    SetValue(key: Tkey, value: any): boolean {
        var index = this.keys.indexOf(key, 0);
        if (index != -1) {
            this.keys[index] = key;
            this.values[index] = value;
            return true;
        }
        return false;
    }

    ContainsKey(key: Tkey): boolean {
        let ks = this.keys;
        for (let i = 0; i < ks.length; ++i) {
            if (ks[i] === key) return true;
        }
        return false;
    }
}