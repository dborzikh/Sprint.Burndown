<template>
    <div class="ui raised link card" v-on:click="$emit('boardClick', board.id)">
        <div class="content">
            <div class="left floated header">{{board.name}}</div>
        </div>
        <div class="content">
            <div class="description">
                <div>
                    ID: <strong>{{board.id}}</strong>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ['board'],
    methods: {
    }
}
</script>

<style>
</style>
