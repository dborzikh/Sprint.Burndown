<template>
    <div class="ui mini modal" id="modalDialog">
        <div class="header">
            {{title}}
        </div>
        <div class="content">
            <p>{{text}}</p>
            <div class="ui checkbox">
                <input type="checkbox" name="updateAllIssues" v-model="updateAllIssues">
                <label>Update all issues</label>
            </div>
        </div>

        <div class="actions">
            <div class="ui primary approve button">
                OK
            </div>
            <div class="ui basic deny button">
                Cancel
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data: function () {
        return {
            element: null,
            updateAllIssues: false
        };
    },

    props: ['title', 'text', 'issuesCount'],

    mounted() {
        $('.ui.checkbox').checkbox();
    },

    methods: {
        show() {
            const self = this;
            self.updateAllIssues = self.issuesCount == 0;

            let promise = new Promise(function (resolve, reject) {
                $('#modalDialog')
                    .modal({
                        inverted: true,
                        onApprove: function () {
                            resolve({ updateAllIssues: self.updateAllIssues });
                            return true;
                        },
                        onDeny: function () {
                            reject();
                            return true;
                        }
                    })
                    .modal('show');
            });

            return promise;
        }
    }
}
</script>

<style>
    .ui.mini.modal {
        top: 40%;
        bottom: initial;
        margin: 1rem auto !important;
    }
</style>
