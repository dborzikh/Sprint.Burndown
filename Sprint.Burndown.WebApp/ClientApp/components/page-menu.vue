<template>
    <div class="ui right floated main menu">
        <a class="undo popup icon item" data-content="Refresh List" v-on:click="$emit('reloadView')">
            <i class="undo icon"></i>
        </a>

        <a class="filter popup icon item" data-content="Filter Sprints">
            <i class="filter icon"></i>
        </a>

    </div>
</template>

<script>
export default {
    data() {
        return {
        }
    },
}
</script>

<style>
</style>
