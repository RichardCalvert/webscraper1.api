<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationvue">https://aka.ms/jspsintegrationvue</a> for more details.
        </div>
        <div class="bd-example">

            <form>
                <div class="mb-3 form-group">
                    <label for="SearchTermInput">Enter search term</label>
                    <input id="searchTermInput" v-on:input="addSearchTerm" class="form-control" />
                </div>
                <div class="mb-3 form-group" :class="{invalid: urlValidity === 'invalid'}">
                    <label for="URLInput">Enter search URL</label>
                    <input id="URLInput" v-on:input="addSearchURL" v-on:blur="validateURL" class="form-control" />
                    <p class="invalid-message hidden" :class="{invalid: urlValidity === 'visible'}" v-if="urlValidity=='invalid'">Please enter a valid URL</p>
                    <!--v-on:input="addSearchURL"-->
                </div>
                <div class="mb-3">
                    <button v-on:click="fetchData" type="button" class="form-control btn btn-primary">Fetch data</button>
                </div>
            </form>
        </div>
        <div v-if="post" class="content">
            <table class="table">
                <thead>
                    <tr>
                        <th>Href</th>
                        <th>Text Content</th>
                        <th>Rank</th>
                        <!--<th>Temp. (F)</th>
                        <th>Summary</th>-->
                    </tr>
                </thead>
                <tbody>
                    <!--<tr v-for="forecast in post" :key="forecast.date">-->
                    <tr v-for="searchResult in post">
                        <td>{{ searchResult.hrefItem }}</td>
                        <td>{{ searchResult.textContent }}</td>
                        <td>{{ searchResult.ranking }}</td>
                    </tr>
                </tbody>
            </table>
        </div>

    </div>
</template>

<script lang="js">
    import Vue from 'vue';

    export default Vue.extend({
        data() {
            return {
                loading: false,
                post: null,
                searchTerm: '',
                searchURL: '',
                urlValidity: 'pending'
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            //this.fetchData();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData'
        },
        methods: {
            addSearchTerm(event) {
                this.searchTerm = event.target.value
            },
            addSearchURL(event) {
                this.searchURL = event.target.value
            },
            validateURL() {
                const urlR = /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g
                if (this.searchURL.trim().match(urlR)) {
                    this.urlValidity = 'valid'
                } else { this.urlValidity = 'invalid' }
            },
            //fetchData() {
            //    this.post = null;
            //    this.loading = true;
            //    axios.post('weatherforecasttwo', { searchTerm: this.searchTerm })
            //        .then(response => Console.log(response.data.id))
            //        .catch(error => {
            //            console.log(`Error: ${error.message}`);
            //        });
            //}
            fetchData() {
                this.post = null;
                this.loading = true;
                fetch('weatherforecasttwo', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        searchTerm: this.searchTerm,
                        searchURL: this.searchURL
                    })
                })
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                        return;
                    })
                    .catch((error) => {
                        console.log(error);
                    });
            }
        }
    });
</script>