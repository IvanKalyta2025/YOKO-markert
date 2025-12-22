using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;


namespace Api.Repositories
{
    public interface IProfileRepository
    {
        Task AddAsync(Profile profile);
        Task<Profile?> GetByUserIdAsync(Guid userId);
        Task UploadFileAsync(string objectName, Stream fileData);

    }
}



// void Add(Profile profile): Этот метод позволит тебе взять данные из формы (имя, фамилия, ссылка на фото) и сохранить их в PostgreSQL.

// Profile? GetByUserId(Guid userId): Это ключевой метод для твоей будущей HTML-страницы. Когда пользователь зайдет в свой профиль, приложение возьмет его Id и с помощью этого метода вытащит из базы именно его данные (имя и аватарку).