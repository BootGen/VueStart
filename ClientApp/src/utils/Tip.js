
export default class Tip {
    constructor() {
        this.tipIdx = parseInt(localStorage.getItem('tipIdx')) || 0;
        this.tips = [
            'Try to edit the JSON data, and see the changes in the application.',
            'If you make structural changes, the application is regenerated.',
            'Try out multiple layouts with the button in the bottom right corner.',
            'When you are done, click the download button.'
        ];
    }

    hideTips() {
        this.tipIdx = this.tips.length;
        localStorage.setItem('tipIdx', this.tipIdx.toString());
    }

    modified() {
        if (this.tipIdx === 0) {
            this.tipIdx = 1;
            localStorage.setItem('tipIdx', this.tipIdx.toString());
            return true
        }
        return false
    }
    generated() {
        if (this.tipIdx === 1) {
            this.tipIdx = 2;
            localStorage.setItem('tipIdx', this.tipIdx.toString());
            return true
        }
        return false
    }
    typeChanged() {
        if (this.tipIdx === 2) {
            this.tipIdx = 3;
            localStorage.setItem('tipIdx', this.tipIdx.toString());
            return true
        }
        return false
    }
    downloaded() {
        if (this.tipIdx === 3) {
            this.tipIdx = 4;
            localStorage.setItem('tipIdx', this.tipIdx.toString());
            return true
        }
        return false
    }
    getTip() {
        let msg = this.tips[this.tipIdx];
        if (this.tipIdx === 4) {
            this.tipIdx = 5;
            localStorage.setItem('tipIdx', this.tipIdx.toString());
        }
        return msg;
    }

}