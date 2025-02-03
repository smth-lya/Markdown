const apiUrl = '/api/auth';

async function signUp(email, password) {
    const requestData = { email, password };

    try {
        const response = await fetch(`${apiUrl}/signup`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestData),
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.errors || 'Ошибка регистрации');
        }

        console.log('Пользователь успешно зарегистрирован', data);
        return data;
    } catch (error) {
        console.error('Ошибка:', error);
    }
}

async function signIn(email, password) {
    const requestData = { email, password };

    try {
        const response = await fetch(`${apiUrl}/signin`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestData),
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.errors || 'Ошибка входа');
        }

        localStorage.setItem('accessToken', data.accessToken);

        console.log('Пользователь успешно вошел', data);
        return data;
    } catch (error) {
        console.error('Ошибка:', error);
    }
}

async function refreshToken() {
    try {
        const response = await fetch(`${apiUrl}/refresh`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.errors || 'Ошибка обновления токена');
        }

        localStorage.setItem('accessToken', data.accessToken);

        console.log('Токен обновлен', data);
        return data;
    } catch (error) {
        console.error('Ошибка:', error);
    }
}

async function signOut() {
    try {
        const response = await fetch(`${apiUrl}/signout`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
        });

        if (!response.ok) {
            throw new Error('Ошибка выхода');
        }

        localStorage.removeItem('accessToken');

        console.log('Выход успешен');
        return true;
    } catch (error) {
        console.error('Ошибка:', error);
    }
}
