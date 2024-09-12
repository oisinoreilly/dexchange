export const arrayUnique = (array) => {
    const a = array.concat();
    for (let i = 0; i < a.length; ++i) {
        for (let j = i + 1; j < a.length; ++j) {
            if (a[i] === a[j])
                a.splice(j--, 1);
        }
    }
    return a;
};
export const generateId = () => {
    const min = 0, max = 999999;
    return String(Math.floor(Math.random() * (max - min + 1)) + min);
};
export const toLocalDateStringUS = (dateString) => {
    const date = new Date(dateString);
    const now = (new Date());
    let options = {};
    if (date.getFullYear() === now.getFullYear())
        options = { month: 'short', day: 'numeric', hour: "2-digit", minute: "2-digit", hour12: false };
    else {
        options = { year: "numeric", month: 'short', day: 'numeric', hour: "2-digit", minute: "2-digit", hour12: false };
    }
    return date.toLocaleDateString('en-US', options);
};
//# sourceMappingURL=Utils.js.map