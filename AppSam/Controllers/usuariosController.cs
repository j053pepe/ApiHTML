using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AppSam.Models;
using AppSam.Clases;
using Newtonsoft.Json.Linq;
using AppSam.DTO;

namespace AppSam.Controllers
{
    public class usuariosController : ApiController
    {
        private AppSamEntities db = new AppSamEntities();

        [HttpGet]
        [ActionName("TraerUsuario")]
        public IHttpActionResult Usuario(string email)
        {
            return Ok(db.usuarios
                        .Where(u => u.Email.Contains(email))
                        .Select(u => new DTOUsuario
                        {
                            Email = u.Email,
                            Paterno = u.Paterno,
                            Materno = u.Materno,
                            Nombre = u.Nombre,
                            usuariosId = u.usuariosId
                        })
                        .FirstOrDefault());
        }

        [HttpGet]
        [ActionName("Login")]
        public IHttpActionResult UsuarioPass(string email, string pass, string IP)
        {
            string Respuesta = null;
            try
            {                
                usuarios usuario = db.usuarios
                            .Where(u => u.Email.Contains(email)).FirstOrDefault();
                if((usuario?.usuariosId ?? 0) > 0)
                {
                    string key = Seguridad.DecryptKey(usuario.llave, usuario.usuariosId.ToString());
                    if (usuario.pass.Contains(Seguridad.EncryptKey(pass, key)))
                    {
                        Respuesta = Seguridad.EncryptKey(usuario.usuariosId.ToString(), IP + DateTime.Now.ToShortDateString() + DateTime.Now.TimeOfDay.ToString());
                        #region Token de Acceso
                        UsuarioToken Token = db.UsuarioToken
                                                    .Where(o =>
                                                            o.UsuarioId == usuario.usuariosId
                                                            && o.EstatusId == 1)
                                                    .FirstOrDefault() ?? null;
                        if (Token == null)
                        {
                            db.UsuarioToken.Add(new UsuarioToken
                            {
                                IP = IP,
                                Token = Respuesta,
                                UsuarioId = usuario.usuariosId,
                                EstatusId = 1,
                                UsuarioAcceso =
                                new List<UsuarioAcceso> {
                                    new UsuarioAcceso
                                    {
                                    FechaIngreso = DateTime.Now,
                                    HoraIngreso = DateTime.Now.TimeOfDay,
                                    UsuarioId = usuario.usuariosId
                                    }
                                }
                            });

                        }
                        else if (Token.Token != Respuesta)
                        {
                            UsuarioToken tokenb =
                               db.UsuarioToken.Where(t => t.UsuarioTokenId == Token.UsuarioTokenId).FirstOrDefault();
                            tokenb.EstatusId = 2;
                            db.UsuarioToken.Add(new UsuarioToken
                            {
                                IP = IP,
                                Token = Respuesta,
                                UsuarioId = usuario.usuariosId,
                                EstatusId = 1,
                                UsuarioAcceso =
                                new List<UsuarioAcceso> {
                                    new UsuarioAcceso
                                    {
                                    FechaIngreso = DateTime.Now,
                                    HoraIngreso = DateTime.Now.TimeOfDay,
                                    UsuarioId = usuario.usuariosId
                                    }
                                }
                            });
                            db.SaveChanges();
                        }
                        else if (Token.Token == Respuesta)
                        {
                            UsuarioToken tokenb =
                                  db.UsuarioToken.Where(t => t.UsuarioTokenId == Token.UsuarioTokenId).FirstOrDefault();
                            tokenb.EstatusId = 2;
                            db.UsuarioToken.Add(new UsuarioToken
                            {
                                IP = IP,
                                Token = Respuesta,
                                UsuarioId = usuario.usuariosId,
                                EstatusId = 1,
                                UsuarioAcceso =
                                new List<UsuarioAcceso> {
                                    new UsuarioAcceso
                                    {
                                    FechaIngreso = DateTime.Now,
                                    HoraIngreso = DateTime.Now.TimeOfDay,
                                    UsuarioId = usuario.usuariosId
                                    }
                                }
                            });
                        }
                        db.SaveChanges();
                        #endregion
                    }
                }
            }
            catch { }
            return Ok(Respuesta); 

        }

        [HttpPut]
        [ActionName("usuario")]
        public void Eliminar(string Id)
        {
            UsuarioToken usuario =
            db.UsuarioToken.Where(u => u.Token == Id && u.EstatusId==1).FirstOrDefault() ?? null;

            if(usuario != null)
            {
                usuario.EstatusId = 2;
                db.SaveChanges();
            }
        }

        [HttpGet]
        [ActionName("TraerMenu")]        
        public IHttpActionResult MenuUsuario(string UsuarioId)
        {
            List<DTOMenu> Menu = null;
            try
            {
                usuarios usuariomenu = db.UsuarioToken.Where(T => T.Token == UsuarioId && T.EstatusId == 1)
                                                        .ToList()?
                                                        .Last()?.usuarios;
               List<Pantallas> Pantallas = db.UsuariosPantallas
                                        .Where(u => u.UsuarioId == usuariomenu.usuariosId)
                                        .Select(u=> u.Pantallas)
                                        .ToList();
                if (Pantallas.Count > 0)
                {
                    Menu = Pantallas
                            .Select(P => P.MenuTitulo)
                            .GroupBy(o => o.MenuTituloId)
                            .Select(l => l.FirstOrDefault())
                            .Select(P => new DTOMenu
                            {
                                Descripcion = P.Descripcion,
                                MenuTituloId = P.MenuTituloId,
                                Icono=P.Icono
                            })
                            .ToList();

                    Menu.ForEach(M =>
                    {
                        M.Pantallas = Pantallas
                                        .OrderBy(O=> O.Descripcion)
                                        .Where(P => P.MenuTituloId == M.MenuTituloId)
                                        .Select(P => new DTOPantalla
                                        {
                                            Descripcion = P.Descripcion,
                                            DireccionPantalla = P.DireccionPantalla,
                                            PantallaId = P.PantallaId,
                                            Icono = P.Icono
                                        })
                                        .ToList();

                    });
                }
            }
            catch { }
            return Ok(Menu);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool usuariosExists(int id)
        {
            return db.usuarios.Count(e => e.usuariosId == id) > 0;
        }
    }
}