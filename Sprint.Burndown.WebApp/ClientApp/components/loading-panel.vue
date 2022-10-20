<template>
    <div class="ui vertical stripe segment" v-show="visible">
        <div class="ui middle aligned stackable grid container">
            <div class="row loading-content" style="">
                <div class="ui active inverted dimmer">
                    <div class="ui text loader">Loading...</div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>

export default {
    data: function () {
        return { };
    },

    props: ['visible'],
}
</script>

<style>
    .ui.grid.container .loading-content {
        min-height: 85vh;
    }
</style>
