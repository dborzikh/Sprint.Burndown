import Vue from 'vue'

export default {
    options: {
        loading: true
    },

    loading(arg1, arg2) {
        if (!this.options.loading) {
            return;
        }

        console.info('[Loading] ' + arg1, arg2);
    }
}