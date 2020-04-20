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

class CreateTimer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            name: '',
            description: '',
            period: 0
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        this.setState({
            [event.target.name]: event.target.value
        });
    }

    handleSubmit(event) {
        const name = this.state.name;
        if (name.length > 64 || name.length < 3) {
            alert("Название должно содержать от 3х до 64х символов");
            return;
        }
        const description = this.state.description;
        if (description.length > 100) {
            alert("Описание должно содержать не больше 100 символов");
            return;
        }
        const period = parseInt(this.state.period);
        if (period < 30 || period > 604800) {
            alert("Период должен быть не меньше 30 секунд и не больше одной недели");
            return;
        }

        event.preventDefault();

        fetch("/TimerApi/CreateTimer", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json;charset=utf-8'
                },
                body: JSON.stringify({name: name, description: description, period: period})
            })
            .then(res => {
                if (res.status === 201){
                    this.props.onSubmit();
                }
            });
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                <label>
                    Название:
                    <input type="text" maxLength="64" name="name" value={this.state.name} onChange={this.handleChange}/>
                </label>
                <label>
                    Описание:
                    <input type="text" maxLength="100" name="description" value={this.state.description}
                           onChange={this.handleChange}/>
                </label>
                <label>
                    Период (секунд):
                    <input type="number" name="period" value={this.state.period} onChange={this.handleChange}/>
                </label>
                <input type="submit" value="Submit"/>
            </form>
        );
    }
}

class Timer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            name: props.initial.Name,
            descripition: props.initial.Description,
            lastUpdate: Math.round(props.initial.LastUpdate / 10000000),
            elapsedTime: props.initial.ElapsedSeconds,
            status: props.initial.State
        };
    }

    tick() {
        if (this.state.status !== 1 && this.state.status !== 3) {
            return;
        }

        let status = this.state.status;
        let lastUpdate = this.state.lastUpdate;
        const elapsedTime = this.state.elapsedTime + 1;
        if (status === 1 && elapsedTime >= this.props.initial.PeriodInSeconds) {
            this.onTimeExpired();
            status = 3;
            lastUpdate = (Math.round(Date.now() / 1000) + 62135596800);
        }

        this.setState({
            name: this.state.name,
            descripition: this.state.descripition,
            lastUpdate: lastUpdate,
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
                    this.props.refresh();
                }
            })
    }

    onTimeExpired() {
        this.sendDoActionRequest(this.props.id, "Expire");
        alert("It's time for '" + this.props.initial.Name + "'");
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
            {this.state.status !== 1 && this.state.status !== 3
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
                       refresh={() => this.refreshTimers()}
                       initial={timerProps}
        />);
    }

    componentDidMount() {
        this.getTimers();
    }

    getTimers() {
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
                });
    }

    refreshTimers() {
        this.setState({
            isLoaded: false,
            timers: null
        });
        this.getTimers();
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
        return (
            <div className="timers-list">
                <CreateTimer onSubmit={() => this.refreshTimers()}/>
                {status}
            </div>);
    }
}

ReactDOM.render(
    <TimerList/>,
    document.getElementById("content")
);