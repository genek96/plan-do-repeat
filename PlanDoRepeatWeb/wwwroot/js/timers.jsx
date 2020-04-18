function Play(props) {
    return (<div className="timer-button timer-button-play" onClick={props.onClick}/>);
}

function Pause(props) {
    return (<div className="timer-button timer-button-pause" onClick={props.onClick}/>);
}

function Stop(props) {
    return (<div className="timer-button timer-button-stop" onClick={props.onClick}/>);
}

function Repeat(props) {
    return (<div className="timer-button timer-button-repeat" onClick={props.onClick}/>);
}

function Delete(props) {
    return (<div className="timer-button timer-button-delete" onClick={props.onClick}/>);
}

class Timer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            name: props.initial.Name,
            descripition: props.initial.Description,
            lastUpdate: Math.round(props.initial.LastUpdate / 10000000),
            elapsedTime: props.initial.PassedSeconds,
            status: props.initial.State
        };
    }

    tick() {
        if (this.state.status !== 1 && this.state.status !== 3) {
            return;
        }

        let status = this.state.status;
        const elapsedTime = this.state.elapsedTime + 1;
        if (status === 1 && elapsedTime >= this.props.initial.PeriodInSeconds) {
            this.onTimeExpired();
            status = 3;
        }

        this.setState({
            name: this.state.name,
            descripition: this.state.descripition,
            lastUpdate: this.state.lastUpdate,
            elapsedTime: this.state.elapsedTime + 1,
            status: status
        });
    }

    componentDidMount() {
        this.interval = setInterval(() => this.tick(), 1000);
    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }

    onClickPlay() {
        if (this.state.status === 1 || this.state.status === 3) {
            return;
        }

        this.sendDoActionRequest(this.props.id, "Start")
            .then(res => {
                if (res) {
                    this.setState({
                        name: this.state.name,
                        descripition: this.state.descripition,
                        lastUpdate: Date.now(),
                        elapsedTime: this.state.elapsedTime,
                        status: 1
                    });
                }
            });
    }

    onClickRepeat() {
        if (this.state.status !== 1 && this.state.status !== 3) {
            return;
        }

        this.sendDoActionRequest(this.props.id, "Repeat")
            .then(res => {
                if (res) {
                    this.setState({
                        name: this.state.name,
                        descripition: this.state.descripition,
                        lastUpdate: Date.now(),
                        elapsedTime: 0,
                        status: 1
                    });
                }
            });
    }

    onClickStop() {
        if (this.state.status === 0) {
            return;
        }

        this.sendDoActionRequest(this.props.id, "Stop")
            .then(res => {
                this.setState({
                    name: this.state.name,
                    descripition: this.state.descripition,
                    lastUpdate: Date.now(),
                    elapsedTime: 0,
                    status: 0
                });
            });
    }

    onClickPause() {
        if (this.state.status === 2) {
            return;
        }

        this.sendDoActionRequest(this.props.id, "Pause")
            .then(res => {
                if (res) {
                    this.setState({
                        name: this.state.name,
                        descripition: this.state.descripition,
                        lastUpdate: Date.now(),
                        elapsedTime: this.state.elapsedTime > 0 ? this.state.elapsedTime : 0,
                        status: 2
                    });
                }
            });
    }

    onClickDelete() {
        if (!confirm("Вы уверены, что хотите удалить таймер: \"" + this.state.name + "\"")) {
            return;
        }

        this.sendDoActionRequest(this.props.id, "Delete")
            .then(res => {
                if (res) {
                    //todo update timers list
                }
            })
    }

    onTimeExpired() {
        this
            .sendDoActionRequest(this.props.id, "Expire")
            .then(res => alert("It's time for '" + this.props.initial.Name + "'"));
    }

    sendDoActionRequest(id, action) {
        return fetch("/TimerApi/DoActionOnTimer?timerId=" + id + "&action=" + action,
            {method: 'POST'})
            .then(res => res.status < 300);
    }

    render() {
        const elapsedSeconds = this.state.status === 3
            ? this.state.lastUpdate - (Math.round(Date.now() / 1000) + 62135596800)
            : this.props.initial.PeriodInSeconds - this.state.elapsedTime;
        
        return (<div className="timer">
            <div className="timer-body">{this.state.name} : {this.state.descripition} : {elapsedSeconds}</div>
            <Repeat onClick={() => this.onClickRepeat()}/>
            {this.state.status !== 1
                ? <Play onClick={() => this.onClickPlay()}/>
                : <Pause onClick={() => this.onClickPause()}/>}
            <Stop onClick={() => this.onClickStop()}/>
            <Delete onClick={() => this.onClickDelete()}/>
        </div>);
    }
}

class TimerList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            timers: []
        };
    }

    renderTimer(timerProps) {
        return (<Timer key={timerProps.Id}
                       id={timerProps.Id}
                       initial={timerProps}
        />);
    }

    componentDidMount() {
        fetch("/TimerApi/GetAllTimers")
            .then(res => res.json())
            .then(
                result => {
                    this.setState({
                        error: null,
                        isLoaded: true,
                        timers: result
                    });
                },
                (error) => {
                    this.setState({
                        error: error.message,
                        isLoaded: true,
                        timers: null
                    });
                })
    }

    render() {
        let status = "";
        if (!this.state.isLoaded) {
            status = "Загрузка...";
        } else if (this.state.error) {
            status = this.state.error;
        } else {
            status = this.state.timers.map(item => this.renderTimer(item));
        }
        return (<div className="timers-list">{status}</div>);
    }
}

ReactDOM.render(
    <TimerList/>,
    document.getElementById("content")
);